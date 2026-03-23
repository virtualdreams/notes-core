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
