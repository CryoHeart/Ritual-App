-- ============================================================
-- RITUAL Migration 001: Add Albums + Song Album Fields
-- Run against: ritual_db
-- ============================================================

-- 1. Create albums table
CREATE TABLE IF NOT EXISTS albums (
    album_id       CHAR(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    band_id        CHAR(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    title          VARCHAR(150) NOT NULL,
    release_year   INT NULL,
    sort_order     INT NOT NULL DEFAULT 0,
    created_at     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at     DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (album_id),
    CONSTRAINT fk_albums_band FOREIGN KEY (band_id) REFERENCES bands(band_id) ON DELETE CASCADE
);

CREATE INDEX idx_albums_band_id ON albums(band_id);

-- 2. Add album columns to songs
ALTER TABLE songs
    ADD COLUMN album_id           CHAR(36) CHARACTER SET ascii COLLATE ascii_general_ci NULL,
    ADD COLUMN album_track_number INT NULL;

ALTER TABLE songs
    ADD CONSTRAINT fk_songs_album FOREIGN KEY (album_id) REFERENCES albums(album_id) ON DELETE SET NULL;

CREATE INDEX idx_songs_album_id ON songs(album_id);

-- ============================================================
-- Seed: Wolf's Cry album for Orpheus Blade
-- ============================================================

-- Insert album (use a deterministic UUID so re-running is safe)
INSERT INTO albums (album_id, band_id, title, release_year, sort_order, created_at)
SELECT
    'a1000000-0000-0000-0000-000000000001',
    b.band_id,
    'Wolf''s Cry',
    NULL,
    1,
    NOW()
FROM bands b
WHERE b.name = 'Orpheus Blade'
ON DUPLICATE KEY UPDATE title = VALUES(title);

-- Assign songs to album with track numbers
-- Track 1: My Red Obsessions (5:53 = 353s)
UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 1
WHERE title = 'My Red Obsessions'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

-- Track 2: The Becoming (4:34 = 274s)
UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 2
WHERE title = 'The Becoming'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

-- Track 3: The Finest Art of Feeding (7:10 = 430s)
UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 3
WHERE title = 'The Finest Art of Feeding'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

-- Track 4: Shadows Still (1:22 = 82s)
UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 4
WHERE title = 'Shadows Still'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

-- Track 5: My Arms for Those Wings (5:35 = 335s)
UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 5
WHERE title = 'My Arms for Those Wings'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

-- Track 6: Nicanor (5:37 = 337s)
UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 6
WHERE title = 'Nicanor'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

-- Track 7: Those Who Cannot Speak (6:28 = 388s)
UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 7
WHERE title = 'Those Who Cannot Speak'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);
