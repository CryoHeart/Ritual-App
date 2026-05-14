-- Migration 005: Band roles and live session ownership
-- Run this against the ritual database.
-- Safe to re-run: each ALTER is guarded by IF NOT EXISTS / IF column not exists.

-- 1. Add started_by_user_id to live_sessions (idempotent via column check)
SET @col_exists = (
    SELECT COUNT(*)
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'live_sessions'
      AND COLUMN_NAME  = 'started_by_user_id'
);

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE live_sessions ADD COLUMN started_by_user_id CHAR(36) CHARACTER SET ascii COLLATE ascii_general_ci NULL AFTER setlist_id',
    'SELECT 1'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 2. Add FK constraint (idempotent via constraint check)
SET @fk_exists = (
    SELECT COUNT(*)
    FROM information_schema.TABLE_CONSTRAINTS
    WHERE TABLE_SCHEMA      = DATABASE()
      AND TABLE_NAME        = 'live_sessions'
      AND CONSTRAINT_NAME   = 'fk_live_sessions_started_by_user'
);

SET @sql2 = IF(@fk_exists = 0,
    'ALTER TABLE live_sessions ADD CONSTRAINT fk_live_sessions_started_by_user FOREIGN KEY (started_by_user_id) REFERENCES users(user_id) ON DELETE SET NULL',
    'SELECT 1'
);
PREPARE stmt2 FROM @sql2; EXECUTE stmt2; DEALLOCATE PREPARE stmt2;

-- 3. Add composite index (idempotent via index check)
SET @idx_exists = (
    SELECT COUNT(*)
    FROM information_schema.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME   = 'live_sessions'
      AND INDEX_NAME   = 'idx_live_sessions_band_status'
);

SET @sql3 = IF(@idx_exists = 0,
    'CREATE INDEX idx_live_sessions_band_status ON live_sessions(band_id, status)',
    'SELECT 1'
);
PREPARE stmt3 FROM @sql3; EXECUTE stmt3; DEALLOCATE PREPARE stmt3;

-- 4. Migrate old 'Admin' role to 'RitualLeader'
UPDATE band_members
SET role = 'RitualLeader'
WHERE role = 'Admin';
