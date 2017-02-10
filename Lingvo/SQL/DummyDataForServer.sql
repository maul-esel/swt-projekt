DELETE FROM Pages;
DELETE FROM Workbooks;
DELETE FROM Recordings;
DELETE FROM Editors;

INSERT INTO Workbooks(id, title, subtitle, isPublished) VALUES
(1, 'Deutschkurs für Asylbewerber', 'Thannhauser Modell', 1),
(2, 'Willkommen! Die deutsche Sprache - erste Schritte', 'Flüchtlingshilfe München e.V.', 1),
(3, 'Test1', '1', 1),
(4, 'Test2', '2', 1),
(5, 'Test3', '3', 1),
(6, 'Test4', '4', 1),
(7, 'Test5', '5', 1),
(8, 'Test6', '6', 1),
(9, 'Test7', '7', 1),
(10, 'Test8', '8', 1),
(11, 'Test9', '9', 1),
(12, 'Test10', '10', 1),
(13, 'Test11', '11', 1),
(14, 'Test12', '12', 1);

INSERT INTO Recordings (id, duration, localPath) VALUES
(1, 127000, 'audios/diktat_seite_01.mp3'),
(2, 462000, 'audios/diktat_seite_02.mp3'),
(3, 127000, 'audios/diktat_seite_03.mp3'),
(4, 368000, 'audios/diktat_seite_04.mp3');

INSERT INTO Pages(id, workbookID, number, description, teacherTrackId) VALUES
(1, 1, 1, 'Begrüßung, Vorstellung und Familie', 1),
(2, 1, 2, 'Wie geht es Ihnen?', 2),
(3, 2, 1, 'Das Alphabet', 3),
(4, 2, 2, 'Erste Gespräche', 4),
(5, 1, 3, 'Test1', 2),
(6, 1, 4, 'Test2', 2),
(7, 1, 5, 'Test3', 2),
(8, 1, 6, 'Test4', 2),
(9, 1, 7, 'Test5', 2),
(10, 1, 8, 'Test6', 2),
(11, 1, 9, 'Test7', 2),
(12, 1, 10, 'Test8', 2),
(13, 1, 11, 'Test9', 2),
(14, 1, 12, 'Test10', 2),
(15, 1, 13, 'Test11', 2),
(16, 1, 14, 'Test12', 2);

INSERT INTO Editors(name, passwordHash) VALUES
("DEV", "AQAAAAEAACcQAAAAEObagOrjXY6h0oymHQWd9na59TRLBdb9etP31ZWMN5OfbCopKZazN4HH59t1SELvPA=="),
("GIETMANN", "AQAAAAEAACcQAAAAEM75qbd85ybQJzWImNGHOoJ7gEu59ANQ2aFf1E86+uCA9M4deXUDixFOTHMfFjUSQQ==");
