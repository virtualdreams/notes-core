# Notes - Docker

Dockerfile is found in root of repository.

## Build image

```sh
docker build --no-cache -t notes-core .
```

## Run image

```sh
docker run \
	-it \
	--rm \
	-p 5000:5000 \
	-e NOTES__ConnectionStrings__PgSql='Host=<PostgresServer>;Database=notes;Username=notes;Password=notes' \
	# -v $(pwd)/storage/key-store:/app/key-store \
	notes-core
```