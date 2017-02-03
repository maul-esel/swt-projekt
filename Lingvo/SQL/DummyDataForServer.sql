DELETE FROM Pages;
DELETE FROM Workbooks;
DELETE FROM Recordings;

INSERT INTO Workbooks(id, title, subtitle, totalPages, isPublished) VALUES
(1, 'Deutschkurs für Asylbewerber', 'Thannhauser Modell', 2, 1),
(2, 'Willkommen! Die deutsche Sprache - erste Schritte', 'Flüchtlingshilfe München e.V.', 2, 1);

INSERT INTO Recordings (id, duration, localPath) VALUES
(1, 127000, 'audios/diktat_seite_01.mp3'),
(2, 462000, 'audios/diktat_seite_02.mp3'),
(3, 127000, 'audios/diktat_seite_03.mp3'),
(4, 368000, 'audios/diktat_seite_04.mp3');

INSERT INTO Pages(id, workbookID, number, description, teacherTrackId) VALUES
(1, 1, 1, 'Begrüßung, Vorstellung und Familie', 1),
(2, 1, 2, 'Wie geht es Ihnen?', 2),
(3, 2, 1, 'Das Alphabet', 3),
(4, 2, 2, 'Erste Gespräche', 4);

INSERT INTO Editors(name, passwordHash) VALUES
("DEV", "AQAAAAEAACcQAAAAEObagOrjXY6h0oymHQWd9na59TRLBdb9etP31ZWMN5OfbCopKZazN4HH59t1SELvPA=="),
("GIETMANN", "AQAAAAEAACcQAAAAEM75qbd85ybQJzWImNGHOoJ7gEu59ANQ2aFf1E86+uCA9M4deXUDixFOTHMfFjUSQQ==");
