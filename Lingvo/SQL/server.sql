-- MySQL database on server --

CREATE TABLE IF NOT EXISTS Workbooks (
  id INT AUTO_INCREMENT PRIMARY KEY,
  title TEXT NOT NULL,
  subtitle TEXT,
  totalPages INT NOT NULL,
  lastModified TIMESTAMP NOT NULL ON UPDATE CURRENT_TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  published BOOLEAN NOT NULL
);

CREATE TABLE IF NOT EXISTS Recordings (
  id INT AUTO_INCREMENT PRIMARY KEY,
  creationTime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  length INT NOT NULL,
  localPath TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Pages (
  workbookID INT,
  number INT,
  description TEXT NOT NULL,
  teacherTrack INT NOT NULL,
  studentTrack INT, /* always NULL on server */
  
  PRIMARY KEY (workbookID, number),
  FOREIGN KEY wbid(workbookID) REFERENCES Workbooks(id),
  FOREIGN KEY ttfk(teacherTrack) REFERENCES Recordings(id),
  FOREIGN KEY stfk(studentTrack) REFERENCES Recordings(id)
);

-- only on server --

DROP TRIGGER IF EXISTS updateTotalPages;
CREATE TRIGGER updateTotalPages
AFTER INSERT ON Pages FOR EACH ROW
BEGIN
  UPDATE Workbooks SET totalPages = totalPages + 1 WHERE id = NEW.workbookID;
END;
