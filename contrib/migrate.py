# Script to migrate from MongoDB to MySQL/MariaDB

# requisites needed
#python-pymongo
#python-mysql-connector

import pymongo
import mysql.connector

# init mongodb
mgoClient = pymongo.MongoClient("mongodb://127.0.0.1:27017")
mgoDb = mgoClient["notes"]
mgoNoteCollection = mgoDb["notes"]
mgoUserCollection = mgoDb["user"]

# init mysql
myClient = mysql.connector.connect(host="localhost", user="notes", passwd="notes", database="notes")
myCursor = myClient.cursor()

# get all users
for user in mgoUserCollection.find():
	# values
	username = user["username"]
	password = user["password"]
	displayname = user["displayname"]
	role = user["role"]
	enabled = user["enabled"]
	created = user["created"].strftime('%Y-%m-%d %H:%M:%S')
	
	# statement
	sql = "insert into user (username, password, displayname, role, enabled, created) values(%s, %s, %s, %s, %s, %s)"
	val = (username, password, displayname, role, enabled, created)
	
	# execute
	myCursor.execute(sql, val)
	myClient.commit()
	
	print("Inserted username:", username)

# get all notes
for note in mgoNoteCollection.find():
	# values
	id = 0
	title = note["title"]
	content = note["content"]
	notebook = note["notebook"]
	trash = note["trash"]
	created = note["created"].strftime('%Y-%m-%d %H:%M:%S') if note["created"] is not None else None
	try:
		modified = note["modified"].strftime('%Y-%m-%d %H:%M:%S') if note["modified"] is not None else None
	except KeyError:
		modified = None
	tags = note["tags"]
	
	# statement
	sql = "insert into note (title, content, notebook, trash, created, modified) values (%s, %s, %s, %s, %s, %s)"
	val = (title, content, notebook, trash, created, modified)
	
	# execute
	myCursor.execute(sql, val)
	myClient.commit()
	id = myCursor.lastrowid
	
	print("Inserted note:", title, id)
	
	# extract tags from note
	if tags is not None:
		for tag in tags:
			# statement
			sql = "insert into tag (name, noteid) values (%s, %s)"
			val = (tag, id)
			
			# execute
			myCursor.execute(sql, val)
			myClient.commit()
			
			print("Inserted tag: ", tag, id)
