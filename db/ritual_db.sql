-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: ritual_db
-- ------------------------------------------------------
-- Server version	9.7.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ 'a3d78b6d-4472-11f1-b739-2cfda1accd5f:1-196';

--
-- Table structure for table `albums`
--

DROP TABLE IF EXISTS `albums`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `albums` (
  `album_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `band_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `title` varchar(150) NOT NULL,
  `release_year` int DEFAULT NULL,
  `sort_order` int NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`album_id`),
  KEY `idx_albums_band_id` (`band_id`),
  CONSTRAINT `fk_albums_band` FOREIGN KEY (`band_id`) REFERENCES `bands` (`band_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `albums`
--

LOCK TABLES `albums` WRITE;
/*!40000 ALTER TABLE `albums` DISABLE KEYS */;
INSERT INTO `albums` VALUES ('4063f3e2-d70d-454a-9abb-3436ee17e664','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','Rectal Revelations',2020,1,'2026-05-05 19:57:03',NULL),('a1000000-0000-0000-0000-000000000001','22222222-2222-2222-2222-222222222222','Wolf\'s Cry',NULL,1,'2026-05-05 22:41:18',NULL);
/*!40000 ALTER TABLE `albums` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `band_members`
--

DROP TABLE IF EXISTS `band_members`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `band_members` (
  `band_member_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `band_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `role` varchar(50) NOT NULL,
  `instrument` varchar(100) DEFAULT NULL,
  `joined_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`band_member_id`),
  UNIQUE KEY `uq_band_members_band_user` (`band_id`,`user_id`),
  KEY `idx_band_members_band_id` (`band_id`),
  KEY `idx_band_members_user_id` (`user_id`),
  CONSTRAINT `fk_band_members_band` FOREIGN KEY (`band_id`) REFERENCES `bands` (`band_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_band_members_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `band_members`
--

LOCK TABLES `band_members` WRITE;
/*!40000 ALTER TABLE `band_members` DISABLE KEYS */;
INSERT INTO `band_members` VALUES ('33333333-3333-3333-3333-333333333333','22222222-2222-2222-2222-222222222222','11111111-1111-1111-1111-111111111111','Admin','Guitar','2026-05-04 18:01:46'),('69a3ed28-48bc-11f1-9a6f-2cfda1accd5f','69a343c4-48bc-11f1-9a6f-2cfda1accd5f','2d1b7d03-a80c-4f3d-b2f6-dada495e4e32','Admin','Band Account','2026-05-05 22:55:48'),('69a3ed67-48bc-11f1-9a6f-2cfda1accd5f','69a349ea-48bc-11f1-9a6f-2cfda1accd5f','40b7de8d-6a5a-43d5-80ab-383fbdd2e499','Admin','Band Account','2026-05-05 22:55:48'),('69a3ed78-48bc-11f1-9a6f-2cfda1accd5f','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','609ef478-9ace-45a8-aebd-a0953c7d1214','Admin','Band Account','2026-05-05 22:55:48'),('69a3ed87-48bc-11f1-9a6f-2cfda1accd5f','69a34b62-48bc-11f1-9a6f-2cfda1accd5f','79b013d0-ad02-4123-82fd-635f68120d19','Admin','Band Account','2026-05-05 22:55:48'),('69a3ed95-48bc-11f1-9a6f-2cfda1accd5f','69a34be2-48bc-11f1-9a6f-2cfda1accd5f','e15802f3-c07f-4343-ade7-dcf4b24a3c74','Admin','Band Account','2026-05-05 22:55:48'),('87bba7ac-9741-4b7a-8257-4d0fde0c20a2','81ce7b94-eaa2-46c3-9c12-85369a73104e','f38f32ca-7f92-4356-937d-9ea86f6daabe','Admin','Band Account','2026-05-05 19:50:19'),('fd5bd075-51d2-4764-907d-3ac4782861c8','0df3fd37-93e2-4f79-824c-7544fa7b606a','68847da2-5bdb-46e9-ad4d-fa6ca48eb5f8','Admin','Band Account','2026-05-05 20:10:57');
/*!40000 ALTER TABLE `band_members` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bands`
--

DROP TABLE IF EXISTS `bands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bands` (
  `band_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` varchar(500) DEFAULT NULL,
  `created_by_user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`band_id`),
  KEY `idx_bands_created_by_user_id` (`created_by_user_id`),
  CONSTRAINT `fk_bands_created_by_user` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bands`
--

LOCK TABLES `bands` WRITE;
/*!40000 ALTER TABLE `bands` DISABLE KEYS */;
INSERT INTO `bands` VALUES ('0df3fd37-93e2-4f79-824c-7544fa7b606a','PW Seed 20260505231056',NULL,'68847da2-5bdb-46e9-ad4d-fa6ca48eb5f8','2026-05-05 20:10:57',NULL),('22222222-2222-2222-2222-222222222222','Orpheus Blade','Progressive metal band workspace','11111111-1111-1111-1111-111111111111','2026-05-04 18:01:46',NULL),('69a343c4-48bc-11f1-9a6f-2cfda1accd5f','Test User',NULL,'2d1b7d03-a80c-4f3d-b2f6-dada495e4e32','2026-05-05 22:55:48',NULL),('69a349ea-48bc-11f1-9a6f-2cfda1accd5f','New User',NULL,'40b7de8d-6a5a-43d5-80ab-383fbdd2e499','2026-05-05 22:55:48',NULL),('69a34acc-48bc-11f1-9a6f-2cfda1accd5f','Masturbational Astonishment',NULL,'609ef478-9ace-45a8-aebd-a0953c7d1214','2026-05-05 22:55:48',NULL),('69a34b62-48bc-11f1-9a6f-2cfda1accd5f','Iron Void 20260505224901',NULL,'79b013d0-ad02-4123-82fd-635f68120d19','2026-05-05 22:55:48',NULL),('69a34be2-48bc-11f1-9a6f-2cfda1accd5f','Iron Void 20260505224942',NULL,'e15802f3-c07f-4343-ade7-dcf4b24a3c74','2026-05-05 22:55:48',NULL),('81ce7b94-eaa2-46c3-9c12-85369a73104e','Iron Void 20260505225017',NULL,'f38f32ca-7f92-4356-937d-9ea86f6daabe','2026-05-05 19:50:19',NULL);
/*!40000 ALTER TABLE `bands` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `live_sessions`
--

DROP TABLE IF EXISTS `live_sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `live_sessions` (
  `live_session_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `band_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `setlist_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `status` varchar(30) NOT NULL DEFAULT 'Active',
  `current_position_index` int NOT NULL DEFAULT '0',
  `started_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ended_at` datetime DEFAULT NULL,
  PRIMARY KEY (`live_session_id`),
  KEY `idx_live_sessions_band_id` (`band_id`),
  KEY `idx_live_sessions_setlist_id` (`setlist_id`),
  CONSTRAINT `fk_live_sessions_band` FOREIGN KEY (`band_id`) REFERENCES `bands` (`band_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_live_sessions_setlist` FOREIGN KEY (`setlist_id`) REFERENCES `setlists` (`setlist_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `live_sessions`
--

LOCK TABLES `live_sessions` WRITE;
/*!40000 ALTER TABLE `live_sessions` DISABLE KEYS */;
/*!40000 ALTER TABLE `live_sessions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `setlist_songs`
--

DROP TABLE IF EXISTS `setlist_songs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `setlist_songs` (
  `setlist_song_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `setlist_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `song_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `position_index` int NOT NULL,
  `transition_notes` text,
  `performance_notes` text,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`setlist_song_id`),
  UNIQUE KEY `uq_setlist_position` (`setlist_id`,`position_index`),
  KEY `idx_setlist_songs_setlist_id` (`setlist_id`),
  KEY `idx_setlist_songs_song_id` (`song_id`),
  CONSTRAINT `fk_setlist_songs_setlist` FOREIGN KEY (`setlist_id`) REFERENCES `setlists` (`setlist_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_setlist_songs_song` FOREIGN KEY (`song_id`) REFERENCES `songs` (`song_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `setlist_songs`
--

LOCK TABLES `setlist_songs` WRITE;
/*!40000 ALTER TABLE `setlist_songs` DISABLE KEYS */;
INSERT INTO `setlist_songs` VALUES ('03ea0d45-30a2-4ce7-b4e9-5834c3741b8c','599caac3-e631-42fb-ab6d-7b2cd38e8a9c','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4',3,NULL,NULL,'2026-05-06 09:31:12'),('22362d76-ae59-4571-98a2-541ae09b8655','6909b4a8-dbc0-411e-8dd4-fcce2f669084','c0b10f62-b2ec-4e0f-8f58-e4c96b7ef638',1,NULL,NULL,'2026-05-05 20:03:00'),('4e3953ad-74da-443c-9254-aec4fd0e730d','6909b4a8-dbc0-411e-8dd4-fcce2f669084','b9da154f-6481-405e-a629-448d442b1b03',3,NULL,NULL,'2026-05-05 20:03:00'),('58b24f3e-bdbd-4f27-88d4-ff78c7c7a5d0','599caac3-e631-42fb-ab6d-7b2cd38e8a9c','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6',5,NULL,NULL,'2026-05-06 09:31:21'),('59124231-bd2e-43e9-bb5c-59165e3b281a','599caac3-e631-42fb-ab6d-7b2cd38e8a9c','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5',4,NULL,NULL,'2026-05-06 09:31:21'),('5fd621d6-434f-4b1d-892c-9ec5247188b3','6909b4a8-dbc0-411e-8dd4-fcce2f669084','54ec41fb-70cb-47d2-b7dd-288991620fdc',4,NULL,NULL,'2026-05-05 20:03:00'),('6827285d-0d2e-4786-b083-102368c1dc4b','599caac3-e631-42fb-ab6d-7b2cd38e8a9c','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1',0,NULL,NULL,'2026-05-06 09:31:12'),('7a3a727d-1e4d-46eb-9ee9-b2046e66c4e1','6909b4a8-dbc0-411e-8dd4-fcce2f669084','6a34c193-cb42-4339-a13b-49e3f6eba237',0,NULL,NULL,'2026-05-05 20:03:00'),('99d72045-53fe-458c-b80b-7aaa5ad8f679','599caac3-e631-42fb-ab6d-7b2cd38e8a9c','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2',1,NULL,NULL,'2026-05-06 09:31:12'),('99df9143-36ce-4b4e-891e-fc5eb985d356','599caac3-e631-42fb-ab6d-7b2cd38e8a9c','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3',2,NULL,NULL,'2026-05-06 09:31:12'),('9a30697d-1977-4c76-8d97-a4945d986a43','599caac3-e631-42fb-ab6d-7b2cd38e8a9c','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7',6,NULL,NULL,'2026-05-06 09:31:25'),('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1','44444444-4444-4444-4444-444444444444','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4',0,'Open the ritual with atmosphere and silence.','Keep the stage dark and focused.','2026-05-04 18:01:46'),('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2','44444444-4444-4444-4444-444444444444','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1',1,'Move directly into the first full song.','Strong entrance after the intro.','2026-05-04 18:01:46'),('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3','44444444-4444-4444-4444-444444444444','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2',2,'Short pause before starting.','Watch the transition energy.','2026-05-04 18:01:46'),('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4','44444444-4444-4444-4444-444444444444','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5',3,'Let the previous ending breathe.','Emotional focus.','2026-05-04 18:01:46'),('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5','44444444-4444-4444-4444-444444444444','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6',4,'Prepare for a heavier shift.','Tight rhythm section.','2026-05-04 18:01:46'),('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6','44444444-4444-4444-4444-444444444444','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7',5,'Minimal talking before this song.','Keep the atmosphere tense.','2026-05-04 18:01:46'),('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7','44444444-4444-4444-4444-444444444444','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3',6,'Final song of the ritual.','End big. No rushing.','2026-05-04 18:01:46'),('e1dae84e-b1e7-4b0f-978e-f9b4d8cbb722','6909b4a8-dbc0-411e-8dd4-fcce2f669084','c23ec73e-4e25-4dc8-8630-bac97c1856dc',5,NULL,NULL,'2026-05-05 20:03:00'),('ff98eb26-d132-4f54-a217-136a219d0c64','6909b4a8-dbc0-411e-8dd4-fcce2f669084','4c8ea2b1-7d1a-4d2a-9184-7ba4a587038d',2,NULL,NULL,'2026-05-05 20:03:00');
/*!40000 ALTER TABLE `setlist_songs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `setlists`
--

DROP TABLE IF EXISTS `setlists`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `setlists` (
  `setlist_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `band_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` varchar(150) NOT NULL,
  `description` varchar(500) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`setlist_id`),
  KEY `idx_setlists_band_id` (`band_id`),
  CONSTRAINT `fk_setlists_band` FOREIGN KEY (`band_id`) REFERENCES `bands` (`band_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `setlists`
--

LOCK TABLES `setlists` WRITE;
/*!40000 ALTER TABLE `setlists` DISABLE KEYS */;
INSERT INTO `setlists` VALUES ('44444444-4444-4444-4444-444444444444','22222222-2222-2222-2222-222222222222','Orpheus Blade Full Ritual','Main Orpheus Blade live ritual set','2026-05-04 18:01:46',NULL),('599caac3-e631-42fb-ab6d-7b2cd38e8a9c','22222222-2222-2222-2222-222222222222','Gagarin Festival June','For June festival 40 minutes','2026-05-06 09:30:50',NULL),('6909b4a8-dbc0-411e-8dd4-fcce2f669084','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','New Album Release show',NULL,'2026-05-05 20:02:46',NULL);
/*!40000 ALTER TABLE `setlists` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `song_cues`
--

DROP TABLE IF EXISTS `song_cues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `song_cues` (
  `song_cue_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `song_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `cue_type` varchar(50) NOT NULL,
  `cue_text` text NOT NULL,
  `cue_time_seconds` int DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`song_cue_id`),
  KEY `idx_song_cues_song_id` (`song_id`),
  CONSTRAINT `fk_song_cues_song` FOREIGN KEY (`song_id`) REFERENCES `songs` (`song_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `song_cues`
--

LOCK TABLES `song_cues` WRITE;
/*!40000 ALTER TABLE `song_cues` DISABLE KEYS */;
INSERT INTO `song_cues` VALUES ('cccccccc-cccc-cccc-cccc-ccccccccccc1','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1','Lighting','Deep red wash when the song starts.',0,'2026-05-04 18:01:46'),('cccccccc-cccc-cccc-cccc-ccccccccccc2','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5','Guitar','Check lead tone before the emotional section.',180,'2026-05-04 18:01:46'),('cccccccc-cccc-cccc-cccc-ccccccccccc3','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3','General','Final song. Keep the ending dramatic and controlled.',390,'2026-05-04 18:01:46');
/*!40000 ALTER TABLE `song_cues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `songs`
--

DROP TABLE IF EXISTS `songs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `songs` (
  `song_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `band_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `title` varchar(150) NOT NULL,
  `bpm` int DEFAULT NULL,
  `duration_seconds` int DEFAULT NULL,
  `tuning` varchar(50) DEFAULT NULL,
  `song_key` varchar(50) DEFAULT NULL,
  `notes` text,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `album_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `album_track_number` int DEFAULT NULL,
  PRIMARY KEY (`song_id`),
  KEY `idx_songs_band_id` (`band_id`),
  KEY `idx_songs_album_id` (`album_id`),
  CONSTRAINT `fk_songs_album` FOREIGN KEY (`album_id`) REFERENCES `albums` (`album_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_songs_band` FOREIGN KEY (`band_id`) REFERENCES `bands` (`band_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `songs`
--

LOCK TABLES `songs` WRITE;
/*!40000 ALTER TABLE `songs` DISABLE KEYS */;
INSERT INTO `songs` VALUES ('4c8ea2b1-7d1a-4d2a-9184-7ba4a587038d','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','Hatoot\'s Litterbox',NULL,356,NULL,NULL,'Mrow?','2026-05-05 19:59:34',NULL,'4063f3e2-d70d-454a-9abb-3436ee17e664',3),('54ec41fb-70cb-47d2-b7dd-288991620fdc','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','Drink Turpentine - Piss on bushfire',NULL,299,NULL,NULL,NULL,'2026-05-05 20:01:48',NULL,'4063f3e2-d70d-454a-9abb-3436ee17e664',5),('6a34c193-cb42-4339-a13b-49e3f6eba237','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','New World Anals',NULL,323,NULL,NULL,'Wait for anal discharge','2026-05-05 19:57:41',NULL,'4063f3e2-d70d-454a-9abb-3436ee17e664',1),('a147add5-c4e0-4ce3-a70d-fba5d49dcc91','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','The Grand Prostate Exam',NULL,394,NULL,NULL,'No daddy don\'t touch me there!','2026-05-05 20:04:26','2026-05-05 20:05:31','4063f3e2-d70d-454a-9abb-3436ee17e664',8),('a5c483ee-9410-409a-bf45-46f5d57bfa72','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','Ether Bunny',NULL,343,NULL,NULL,'Guitar is shoved without lube.','2026-05-05 20:05:00','2026-05-05 20:05:27','4063f3e2-d70d-454a-9abb-3436ee17e664',7),('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1','22222222-2222-2222-2222-222222222222','My Red Obsessions',NULL,353,NULL,NULL,'Length: 5:53','2026-05-04 18:01:46','2026-05-05 22:41:18','a1000000-0000-0000-0000-000000000001',1),('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2','22222222-2222-2222-2222-222222222222','The Becoming',NULL,274,NULL,NULL,'Length: 4:34','2026-05-04 18:01:46','2026-05-05 22:41:18','a1000000-0000-0000-0000-000000000001',2),('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3','22222222-2222-2222-2222-222222222222','The Finest Art of Feeding',NULL,430,NULL,NULL,'Length: 7:10','2026-05-04 18:01:46','2026-05-05 22:41:19','a1000000-0000-0000-0000-000000000001',3),('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4','22222222-2222-2222-2222-222222222222','Shadows Still',NULL,82,NULL,NULL,'Length: 1:22','2026-05-04 18:01:46','2026-05-05 22:41:19','a1000000-0000-0000-0000-000000000001',4),('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5','22222222-2222-2222-2222-222222222222','My Arms for Those Wings',NULL,335,NULL,NULL,'Length: 5:35','2026-05-04 18:01:46','2026-05-05 22:41:19','a1000000-0000-0000-0000-000000000001',5),('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6','22222222-2222-2222-2222-222222222222','Nicanor',NULL,337,NULL,NULL,'Length: 5:37','2026-05-04 18:01:46','2026-05-05 22:41:19','a1000000-0000-0000-0000-000000000001',6),('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7','22222222-2222-2222-2222-222222222222','Those Who Cannot Speak',NULL,388,NULL,NULL,'Length: 6:28','2026-05-04 18:01:46','2026-05-05 22:41:19','a1000000-0000-0000-0000-000000000001',7),('b9da154f-6481-405e-a629-448d442b1b03','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','Gastro Carnage',NULL,286,NULL,NULL,'ARGH!','2026-05-05 20:00:24',NULL,'4063f3e2-d70d-454a-9abb-3436ee17e664',4),('c0b10f62-b2ec-4e0f-8f58-e4c96b7ef638','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','Euclidius Vaginismous',NULL,286,NULL,NULL,'If vagina tastes like yeast, pause the show','2026-05-05 19:58:50',NULL,'4063f3e2-d70d-454a-9abb-3436ee17e664',2),('c23ec73e-4e25-4dc8-8630-bac97c1856dc','69a34acc-48bc-11f1-9a6f-2cfda1accd5f','Ten Thousands tongues in my anus',NULL,323,NULL,NULL,'Mmmmm yes...','2026-05-05 20:02:26',NULL,'4063f3e2-d70d-454a-9abb-3436ee17e664',6);
/*!40000 ALTER TABLE `songs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `display_name` varchar(100) NOT NULL,
  `email` varchar(150) NOT NULL,
  `password_hash` varchar(255) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES ('11111111-1111-1111-1111-111111111111','Logan','OrpheusBlade@ritual.local','$2a$11$dd1ak8YlxFhioOjFqhitj.88HjWpYkNjJ3Gdbr.VUAf98LAMLO0im','2026-05-04 18:01:46','2026-05-05 23:11:18'),('2d1b7d03-a80c-4f3d-b2f6-dada495e4e32','Test User','test20260505224027@ritual.local','$2a$11$46Ul9IWSZidXjOUUMzYxU.yqM9wQb6PqBaFqERI5RQkMyEaX2Caw.','2026-05-05 19:40:27',NULL),('40b7de8d-6a5a-43d5-80ab-383fbdd2e499','New User','newuser20260505223552@ritual.local','$2a$11$QSnbnWia8Z3NlTuvKvZeYO2u851aCeibEz2e4c5ZM3fS7s1RrHfBC','2026-05-05 19:35:54',NULL),('609ef478-9ace-45a8-aebd-a0953c7d1214','Masturbational Astonishment','masturbation@ritual.local','$2a$11$hkJppP4GGdes8M4NZOkkxOZ80U9OILnh1hDz5.ac4y5iLeJICxaPG','2026-05-05 19:38:39',NULL),('68847da2-5bdb-46e9-ad4d-fa6ca48eb5f8','PW Seed 20260505231056','pwseed20260505231056@ritual.local','$2a$11$dd1ak8YlxFhioOjFqhitj.88HjWpYkNjJ3Gdbr.VUAf98LAMLO0im','2026-05-05 20:10:57',NULL),('79b013d0-ad02-4123-82fd-635f68120d19','Iron Void 20260505224901','band20260505224901@ritual.local','$2a$11$7MJVHM3pTpW25syErVyfOOn1mAI.cXyKdzLssWpEw8KQC8cF8Kp7e','2026-05-05 19:49:03',NULL),('e15802f3-c07f-4343-ade7-dcf4b24a3c74','Iron Void 20260505224942','band20260505224942@ritual.local','$2a$11$JiyXcSghSoGfqzChQlc2Fe3yUGhLCLqwpEawCADABq4hD6aabTRdW','2026-05-05 19:49:43',NULL),('f38f32ca-7f92-4356-937d-9ea86f6daabe','Iron Void 20260505225017','band20260505225017@ritual.local','$2a$11$O5qPcEnZtb5U78cG7uAcC.8plChclWib9aQJyXIk.ihYi8U04X2x2','2026-05-05 19:50:19',NULL);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-08 17:13:20
