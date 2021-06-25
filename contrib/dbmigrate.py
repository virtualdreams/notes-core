#!/usr/bin/env python3

import mysql.connector as mysql
import psycopg2 as pgsql
from configparser import ConfigParser
import argparse


def parse_args():
    parser = argparse.ArgumentParser()
    parser.add_argument("-i", "--ini", required=False,
                        default="dbmigrate.ini", dest="ini", help="Configuration file.")
    parser.add_argument("-m", "--mysql", required=False,
                        default="mysql", dest="mysql_section", help="MySql section.")
    parser.add_argument("-p", "--pgsql", required=False,
                        default="pgsql", dest="pgsql_section", help="PgSql section.")

    subparsers = parser.add_subparsers(dest="command", title="Converter")

    mysql_to_pgsql_parser = subparsers.add_parser("m2p", help="MySql to PgSql")

    pgsql_to_mysql_parser = subparsers.add_parser("p2m", help="PgSql to MySql")

    return parser.parse_args()


def read_config(filename, section):
    parser = ConfigParser()
    parser.read(filename)

    db = {}
    if parser.has_section(section):
        params = parser.items(section)
        for param in params:
            db[param[0]] = param[1]

    return db


def copy_note(src_connection, dst_connection):
    # delete src
    dst_cursor = dst_connection.cursor()
    dst_cursor.execute("delete from \"note\";")
    dst_connection.commit()
    dst_cursor.close()

    # copy from src
    src_cursor = src_connection.cursor()
    src_cursor.execute(
        "select id, title, content, notebook, trash, created, modified from \"note\";")
    for (id, title, content, notebook, trash, created, modified) in src_cursor:
        print("Insert into \"note\"", id)
        # insert to dst
        dst_cursor = dst_connection.cursor()
        dst_cursor.execute("insert into \"note\" (id, title, content, notebook, trash, created, modified) values (%s, %s, %s, %s, %s, %s, %s);",
                           (id, title, content, notebook, bool(trash), created, modified))
        dst_connection.commit()
        dst_cursor.close()
    src_cursor.close()


def copy_tag(src_connection, dst_connection):
    # delete src
    dst_cursor = dst_connection.cursor()
    dst_cursor.execute("delete from \"tag\";")
    dst_connection.commit()
    dst_cursor.close()

    # copy from src
    src_cursor = src_connection.cursor()
    src_cursor.execute("select id, name, noteid from \"tag\";")
    for (id, name, noteid) in src_cursor:
        print("Insert into \"tag\"", id)
        # insert to dst
        dst_cursor = dst_connection.cursor()
        dst_cursor.execute("insert into \"tag\" (id, name, noteid) values (%s, %s, %s);",
                           (id, name, noteid))
        dst_connection.commit()
        dst_cursor.close()
    src_cursor.close()


def copy_user(src_connection, dst_connection):
    # delete src
    dst_cursor = dst_connection.cursor()
    dst_cursor.execute("delete from \"user\";")
    dst_connection.commit()
    dst_cursor.close()

    # copy from src
    src_cursor = src_connection.cursor()
    src_cursor.execute(
        "select id, username, password, displayname, role, enabled, created, modified, version, items from \"user\";")
    for (id, username, password, displayname, role, enabled, created, modified, version, items) in src_cursor:
        print("Insert into \"user\"", id)
        # insert to dst
        dst_cursor = dst_connection.cursor()
        dst_cursor.execute("insert into \"user\" (id, username, password, displayname, role, enabled, created, modified, version, items) values (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s);",
                           (id, username, password, displayname, role, bool(enabled), created, modified, version, items))
        dst_connection.commit()
        dst_cursor.close()
    src_cursor.close()


def copy_revision(src_connection, dst_connection):
    # delete src
    dst_cursor = dst_connection.cursor()
    dst_cursor.execute("delete from \"revision\";")
    dst_connection.commit()
    dst_cursor.close()

    # copy from src
    src_cursor = src_connection.cursor()
    src_cursor.execute(
        "select id, dt, noteid, title, content, notebook, trash, created, modified from \"revision\";")
    for (id, dt, noteid, title, content, notebook, trash, created, modified) in src_cursor:
        print("Insert into \"revision\"", id)
        # insert to dst
        dst_cursor = dst_connection.cursor()
        dst_cursor.execute("insert into \"revision\" (id, dt, noteid, title, content, notebook, trash, created, modified) values (%s, %s, %s, %s, %s, %s, %s, %s, %s);",
                           (id, dt, noteid, title, content, notebook, bool(trash), created, modified))
        dst_connection.commit()
        dst_cursor.close()
    src_cursor.close()


def psql_set_sequence(dst_connection):
    # note
    print("Set sequence \"note\"")
    dst_cursor = dst_connection.cursor()
    dst_cursor.execute(
        "SELECT setval('note_id_seq', (SELECT MAX(id) from \"note\"));")
    dst_connection.commit()
    dst_cursor.close()

    # tag
    print("Set sequence \"tag\"")
    dst_cursor = dst_connection.cursor()
    dst_cursor.execute(
        "SELECT setval('tag_id_seq', (SELECT MAX(id) from \"tag\"));")
    dst_connection.commit()
    dst_cursor.close()

    # user
    print("Set sequence \"user\"")
    dst_cursor = dst_connection.cursor()
    dst_cursor.execute(
        "SELECT setval('user_id_seq', (SELECT MAX(id) from \"user\"));")
    dst_connection.commit()
    dst_cursor.close()

    # revision
    print("Set sequence \"revision\"")
    dst_cursor = dst_connection.cursor()
    dst_cursor.execute(
        "SELECT setval('revision_id_seq', (SELECT MAX(id) from \"revision\"));")
    dst_connection.commit()
    dst_cursor.close()


def mysql_set_mode(dst_connection):
    dst_cursor = dst_connection.cursor()
    dst_cursor.execute(
        "set sql_mode = 'ANSI_QUOTES';")
    dst_connection.commit()
    dst_cursor.close()


def main():
    # parse arguments
    args = parse_args()

    # get connection data
    mysql_dsn = read_config(args.ini, args.mysql_section)
    pgsql_dsn = read_config(args.ini, args.pgsql_section)

    if args.command == "m2p":
        # open connections
        src_connection = mysql.connect(**mysql_dsn)
        dst_connection = pgsql.connect(**pgsql_dsn)

        # set mysql mode
        mysql_set_mode(src_connection)
    elif args.command == "p2m":
        # open connections
        src_connection = pgsql.connect(**pgsql_dsn)
        dst_connection = mysql.connect(**mysql_dsn)

        # set mysql mode
        mysql_set_mode(dst_connection)
    else:
        print("No converter selected.")
        return 1

    # copy src to dst
    copy_note(src_connection, dst_connection)
    copy_tag(src_connection, dst_connection)
    copy_user(src_connection, dst_connection)
    copy_revision(src_connection, dst_connection)

    if args.command == "m2p":
        psql_set_sequence(dst_connection)

    src_connection.close()
    dst_connection.close()


if __name__ == "__main__":
    main()
