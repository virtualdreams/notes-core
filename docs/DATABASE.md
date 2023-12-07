# Database

## PostgreSQL

Create user.

```sql
create user notes with password 'password';
```

Create database.

```sql
create database notes with owner notes encoding 'UTF8' lc_collate = 'en_US.UTF-8' lc_ctype = 'en_US.UTF-8' template template0;
```

Remove create rights for public.

```sql
\c notes

revoke create on schema public from public; 
grant create on schema public to notes;
```

Import schema.

```sh
psql -U notes -h localhost -d notes < contrib/database-create-psql.sql 
```

## MariaDB

Create user.

```sql
create user 'notes'@'localhost' identified by 'password';
grant all on notes.* to 'notes'@'localhost';
```

Create database.

```sql
create database notes;
```

Import schema.

```sh
mysql -u notes -p -D notes < contrib/database-create-mysql.sql
```
