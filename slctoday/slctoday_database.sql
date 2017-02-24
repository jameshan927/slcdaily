DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
   `id` int(11) NOT NULL AUTO_INCREMENT,
   `name` varchar(255) DEFAULT NULL,
   `email` varchar(255) DEFAULT NULL,
   PRIMARY KEY (`id`)
)  ENGINE=InnoDB;