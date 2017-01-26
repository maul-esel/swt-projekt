CREATE TABLE IF NOT EXISTS Workbooks (
  id INT PRIMARY KEY,
  title TEXT NOT NULL,
  subtitle TEXT,
  totalPages INT NOT NULL,
  lastModified TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, -- see trigger below
  isPublished BOOLEAN NOT NULL
);

CREATE TABLE IF NOT EXISTS Recordings (
  id INT PRIMARY KEY,
  creationTime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  duration INT NOT NULL,
  localPath TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Pages (
  id INT PRIMARY KEY,
  workbookID INT REFERENCES Workbooks ON DELETE CASCADE,
  number INT,
  description TEXT NOT NULL,
  teacherTrackId INT NOT NULL REFERENCES Recordings,
  studentTrackId INT REFERENCES Recordings
);

CREATE TABLE IF NOT EXISTS TeacherMemos (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  recording INT NOT NULL REFERENCES Recordings
);

CREATE TRIGGER IF NOT EXISTS [UpdateLastModified]
  AFTER UPDATE
  ON Workbooks
  FOR EACH ROW
  WHEN NEW.lastModified < OLD.lastModified
BEGIN
  UPDATE Workbooks SET lastModified = CURRENT_TIMESTAMP WHERE id = OLD.id;
END;