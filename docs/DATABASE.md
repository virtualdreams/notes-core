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

Import schema.

```sh
psql -U notes -h localhost -d notes < contrib/Schema/Postgres/schema.sql 
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
mysql -u notes -p -D notes < contrib/Schema/MySql/schema.sql
```
