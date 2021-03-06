-- MySQL database on server --

CREATE TABLE IF NOT EXISTS Workbooks (
  id INT AUTO_INCREMENT PRIMARY KEY,
  title VARCHAR(400) NOT NULL UNIQUE,
  subtitle TEXT,
  totalPages INT NOT NULL DEFAULT 0,
  lastModified TIMESTAMP NOT NULL ON UPDATE CURRENT_TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  isPublished BOOLEAN NOT NULL
);

CREATE TABLE IF NOT EXISTS Recordings (
  id INT AUTO_INCREMENT PRIMARY KEY,
  creationTime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  duration INT NOT NULL,
  localPath TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Pages (
  id INT PRIMARY KEY AUTO_INCREMENT,
  workbookID INT,
  number INT,
  description TEXT,
  teacherTrackId INT NOT NULL,
  studentTrackId INT, /* always NULL on server */
  
  UNIQUE (workbookID, number),

  FOREIGN KEY wbid(workbookID) REFERENCES Workbooks(id) ON DELETE CASCADE,
  FOREIGN KEY ttfk(teacherTrackId) REFERENCES Recordings(id),
  FOREIGN KEY stfk(studentTrackId) REFERENCES Recordings(id)
);

-- only on server --
CREATE TABLE IF NOT EXISTS Editors (
  name varchar(30) PRIMARY KEY,
  passwordHash TEXT NOT NULL
);

DROP TRIGGER IF EXISTS increaseTotalPages;
CREATE TRIGGER increaseTotalPages
AFTER INSERT ON Pages
FOR EACH ROW
UPDATE Workbooks SET totalPages = totalPages + 1 WHERE id = NEW.workbookID;

DROP TRIGGER IF EXISTS decreaseTotalPages;
CREATE TRIGGER decreaseTotalPages
AFTER DELETE ON Pages
FOR EACH ROW
UPDATE Workbooks SET totalPages = totalPages - 1 WHERE id = OLD.workbookID;

DROP TRIGGER IF EXISTS updateTotalPages;
CREATE TRIGGER updateTotalPages
AFTER UPDATE ON Pages
FOR EACH ROW
UPDATE Workbooks SET totalPages = totalPages + (
	CASE id
	WHEN OLD.workbookID THEN -1
	WHEN NEW.workbookID THEN +1
	ELSE 0
	END
)
WHERE OLD.workbookID != NEW.workbookID AND (id = OLD.workbookID OR id = NEW.workbookID);
