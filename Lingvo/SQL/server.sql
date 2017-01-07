-- MySQL database on server --

CREATE TABLE Workbooks (
  id INT AUTO_INCREMENT PRIMARY KEY,
  title TEXT NOT NULL,
  subtitle TEXT,
  lastModified TIMESTAMP NOT NULL ON UPDATE CURRENT_TIMESTAMP, /* client: sqlite syntax */
  published BOOLEAN NOT NULL
);

CREATE TABLE Recordings (
  id INT AUTO_INCREMENT PRIMARY KEY,
  creationTime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, /* client: sqlite syntax */
  length INT NOT NULL,
  localPath TEXT NOT NULL
);

CREATE TABLE Pages (
  workbookID INT REFERENCES Workbooks,
  number INT,
  description TEXT NOT NULL,
  teacherTrack INT NOT NULL REFERENCES Recordings,
  studentTrack INT REFERENCES Recordings, /* always NULL on server */
  
  PRIMARY KEY (workbookID, number)
);