-- ============================================================
-- RITUAL Seed 003: Assign "Wolf's Cry" Album + Track Numbers
-- Run against: ritual_db
-- ============================================================

-- Insert album (deterministic UUID keeps script idempotent)
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
ON DUPLICATE KEY UPDATE title = VALUES(title), sort_order = VALUES(sort_order);

-- Assign songs to album with track numbers
UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 1
WHERE title = 'My Red Obsessions'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 2
WHERE title = 'The Becoming'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 3
WHERE title = 'The Finest Art of Feeding'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 4
WHERE title = 'Shadows Still'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 5
WHERE title = 'My Arms for Those Wings'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 6
WHERE title = 'Nicanor'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);

UPDATE songs
SET album_id = 'a1000000-0000-0000-0000-000000000001', album_track_number = 7
WHERE title = 'Those Who Cannot Speak'
  AND band_id = (SELECT band_id FROM bands WHERE name = 'Orpheus Blade' LIMIT 1);
