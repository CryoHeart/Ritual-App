-- MySQL dump 10.13  Distrib 8.0.27, for Win64 (x86_64)
--
-- Host: localhost    Database: ritual_db
-- ------------------------------------------------------
-- Server version	8.0.27

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

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
  `musicbrainz_release_group_id` varchar(36) DEFAULT NULL,
  `musicbrainz_release_id` varchar(36) DEFAULT NULL,
  `release_year` int DEFAULT NULL,
  `sort_order` int NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`album_id`),
  KEY `idx_albums_band_id` (`band_id`),
  UNIQUE KEY `uq_albums_band_release_group` (`band_id`,`musicbrainz_release_group_id`),
  KEY `idx_albums_band_title` (`band_id`,`title`),
  CONSTRAINT `fk_albums_band` FOREIGN KEY (`band_id`) REFERENCES `bands` (`band_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

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
-- Table structure for table `bands`
--

DROP TABLE IF EXISTS `bands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bands` (
  `band_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` varchar(500) DEFAULT NULL,
  `country` varchar(100) DEFAULT NULL,
  `musicbrainz_artist_id` varchar(36) DEFAULT NULL,
  `created_by_user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`band_id`),
  UNIQUE KEY `uq_bands_musicbrainz_artist_id` (`musicbrainz_artist_id`),
  KEY `idx_bands_created_by_user_id` (`created_by_user_id`),
  CONSTRAINT `fk_bands_created_by_user` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

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
-- Table structure for table `songs`
--

DROP TABLE IF EXISTS `songs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `songs` (
  `song_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `band_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `title` varchar(150) NOT NULL,
  `musicbrainz_recording_id` varchar(36) DEFAULT NULL,
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
  UNIQUE KEY `uq_songs_band_recording` (`band_id`,`musicbrainz_recording_id`),
  KEY `idx_songs_band_title` (`band_id`,`title`),
  KEY `idx_songs_album_id` (`album_id`),
  CONSTRAINT `fk_songs_album` FOREIGN KEY (`album_id`) REFERENCES `albums` (`album_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_songs_band` FOREIGN KEY (`band_id`) REFERENCES `bands` (`band_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

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
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-08 18:52:23
