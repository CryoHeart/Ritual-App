ALTER TABLE bands
  ADD COLUMN country VARCHAR(100) NULL AFTER description,
  ADD COLUMN musicbrainz_artist_id VARCHAR(36) NULL AFTER country,
  ADD UNIQUE KEY uq_bands_musicbrainz_artist_id (musicbrainz_artist_id);

ALTER TABLE albums
  ADD COLUMN musicbrainz_release_group_id VARCHAR(36) NULL AFTER title,
  ADD COLUMN musicbrainz_release_id VARCHAR(36) NULL AFTER musicbrainz_release_group_id,
  ADD UNIQUE KEY uq_albums_band_release_group (band_id, musicbrainz_release_group_id),
  ADD KEY idx_albums_band_title (band_id, title);

ALTER TABLE songs
  ADD COLUMN musicbrainz_recording_id VARCHAR(36) NULL AFTER title,
  ADD UNIQUE KEY uq_songs_band_recording (band_id, musicbrainz_recording_id),
  ADD KEY idx_songs_band_title (band_id, title);
