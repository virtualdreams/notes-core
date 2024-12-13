-- note table
CREATE TABLE `note` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `content` MEDIUMTEXT COLLATE utf8mb4_unicode_ci NOT NULL,
  `notebook` VARCHAR(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `trash` TINYINT(1) NOT NULL DEFAULT 0,
  `created` DATETIME NOT NULL,
  `modified` DATETIME NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_trash` (`trash`),
  KEY `ix_notebook` (`notebook`),
  FULLTEXT KEY `ft_title` (`title`),
  FULLTEXT KEY `ft_content` (`content`),
  FULLTEXT KEY `ft_notebook` (`notebook`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- tag table
CREATE TABLE `tag` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `noteid` INT(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_noteid` (`noteid`),
  FULLTEXT KEY `ft_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- user table
CREATE TABLE `user` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `password` VARCHAR(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `displayname` VARCHAR(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `role` VARCHAR(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `enabled` TINYINT(1) NOT NULL DEFAULT 0,
  `created` DATETIME NOT NULL,
  `modified` DATETIME NOT NULL,
  `version` INT(11) NOT NULL DEFAULT 0,
  `items` INT(11) NOT NULL DEFAULT 10,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- revision table
CREATE TABLE `revision` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `dt` DATETIME NOT NULL,
  `noteid` INT(11) NOT NULL,
  `title` VARCHAR(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `content` MEDIUMTEXT COLLATE utf8mb4_unicode_ci NOT NULL,
  `notebook` VARCHAR(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `trash` TINYINT(1) NOT NULL DEFAULT 0,
  `created` DATETIME NOT NULL,
  `modified` DATETIME NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- create revision trigger and function
CREATE TRIGGER
    `update_note`
BEFORE UPDATE ON
    `note`
FOR EACH ROW
INSERT INTO revision (
    dt,
    noteid,
    title,
    content,
    notebook,
    trash,
    created,
    modified
) VALUES (
    utc_timestamp(),
    old.id,
    old.title,
    old.content,
    old.notebook,
    old.trash,
    old.created,
    old.modified
);