-- schema table
create table schema (
  version int not null,
  applied_on timestamptz not null, 
  description varchar(1024) not null
);

-- note table
create table note (
  id serial primary key,
  title varchar(100) not null,
  content text not null,
  notebook varchar(50) default null,
  trash bool not null default false,
  created timestamptz not null,
  modified timestamptz not null
);

-- tag table
create table tag (
  id serial primary key,
  name varchar(50) not null,
  noteid int not null
);

-- user table
create table "user" (
  id serial primary key,
  username varchar(100) unique not null,
  password varchar(100) not null,
  displayname varchar(50) default null,
  role varchar(50) not null,
  enabled bool not null default false,
  created timestamptz not null,
  modified timestamptz not null,
  version int not null default 0,
  items int not null default 10
);

-- revision table
create table revision (
  id serial primary key,
  dt timestamptz not null,
  noteid int not null,
  title varchar(100) not null,
  content text not null,
  notebook varchar(50) default null,
  trash bool not null default false,
  created timestamptz not null,
  modified timestamptz not null
);

-- create indexes
create index ix_schema_version on schema(version);
create index ix_note_trash on note(trash);
create index ix_note_notebook on note(notebook);
create index ix_tag_noteid on tag(noteid);

-- create revision function
create function create_revision()
returns trigger as
$BODY$
begin
	insert into revision (
		dt,
		noteid,
		title,
		content,
		notebook,
		trash,
		created,
		modified
	) values (
		now(),
		old.id,
		old.title,
		old.content,
		old.notebook,
		old.trash,
		old.created,
		old.modified
	);
	return new;
end;
$BODY$
language plpgsql volatile cost 100;

-- create revision trigger
create trigger 
	create_revision 
before update on 
	note 
for each row execute procedure create_revision();

-- create fulltext index for note.title
create index 
	ft_note_title 
on 
	note 
using gin ( to_tsvector('english', title ));

-- create fulltext index for note.content
create index 
	ft_note_content 
on 
	note 
using gin ( to_tsvector('english', content));

-- create fulltext index for note.notebook
create index 
	ft_note_notebook 
on 
	note 
using gin ( to_tsvector('english', notebook ));

-- create fulltext index for tag.name
create index 
	ft_tag_name 
on 
	tag 
using gin ( to_tsvector('english', name ));

-- insert schema version
insert into schema
  (version, applied_on, description)
  values (1, NOW(), 'Schema create.')
