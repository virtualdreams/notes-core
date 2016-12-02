db = db.getSiblingDB('notes')
db.user.createIndex( { Name: 1 }, { unique: true } )
db.notes.createIndex( { Title: "text", Content: "text", Notebook: "text", Tags: "text" } )