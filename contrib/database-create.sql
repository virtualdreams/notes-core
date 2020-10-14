DROP TABLE IF EXISTS `note`;
CREATE TABLE `note` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `content` mediumtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `notebook` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `trash` tinyint(1) NOT NULL DEFAULT 0,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_trash` (`trash`),
  KEY `ix_notebook` (`notebook`),
  FULLTEXT KEY `ft_title_content_notebook` (`title`,`content`,`notebook`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `tag`;
CREATE TABLE `tag` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `noteid` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_noteid` (`noteid`),
  FULLTEXT KEY `ft_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `password` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `displayname` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `role` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `enabled` tinyint(1) NOT NULL DEFAULT 0,
  `created` datetime NOT NULL,
  `items` int(11) NOT NULL DEFAULT 10,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `revision`;
CREATE TABLE `revision` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `dt` datetime NOT NULL,
  `noteid` int(11) NOT NULL,
  `title` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `content` mediumtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `notebook` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `trash` tinyint(1) NOT NULL DEFAULT 0,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TRIGGER IF EXISTS `update_note`;
CREATE TRIGGER
    `update_note`
before update on
    `note`
for each row
insert into revision (
    dt,
    noteid,
    title,
    content,
    notebook,
    trash,
    created,
    modified
) values(
    utc_timestamp(),
    old.id,
    old.title,
    old.content,
    old.notebook,
    old.trash,
    old.created,
    old.modified
);