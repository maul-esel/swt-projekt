-- SQLite database on client --

CREATE TABLE Workbooks (
  id INT PRIMARY KEY,
  title TEXT NOT NULL,
  subtitle TEXT,
  totalPages INT NOT NULL,
  lastModified TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, -- see trigger below
  published BOOLEAN NOT NULL
);

CREATE TABLE Recordings (
  id INT PRIMARY KEY,
  creationTime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  length INT NOT NULL,
  localPath TEXT NOT NULL
)

CREATE TABLE Pages (
  workbookID INT REFERENCES Workbooks,
  number INT,
  description TEXT NOT NULL,
  teacherTrack INT NOT NULL REFERENCES Recordings,
  studentTrack INT REFERENCES Recordings, -- always NULL on server

  PRIMARY KEY (workbookID, number)
);

-- only on client --
CREATE TABLE TeacherMemos (
  id INT PRIMARY KEY,
  name TEXT NOT NULL,
  recording INT NOT NULL REFERENCES Recordings
);

CREATE TRIGGER [UpdateLastModified]
  AFTER UPDATE
  ON Workbooks
  FOR EACH ROW
  WHEN NEW.lastModified < OLD.lastModified
BEGIN
  UPDATE Workbooks SET lastModified = CURRENT_TIMESTAMP WHERE id = OLD.id;
END;