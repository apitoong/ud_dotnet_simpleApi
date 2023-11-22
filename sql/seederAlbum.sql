-- Menanamkan data dummy ke tabel Artist
INSERT INTO "Artists" ("Id", "Name")
VALUES (1, 'John Doe'),
       (2, 'Jane Smith'),
       (3, 'Bob Johnson'),
       (4, 'Alice Williams'),
       (5, 'Charlie Brown'),
       (6, 'Eva Davis'),
       (7, 'Frank White'),
       (8, 'Grace Miller'),
       (9, 'Henry Jones'),
       (10, 'Ivy Taylor');

-- Menanamkan data dummy ke tabel Album
INSERT INTO "Albums" ("Id", "Title", "ArtistId")
VALUES (1, 'Sample Album 1', 1),
       (2, 'Sample Album 2', 2),
       (3, 'Sample Album 3', 3),
       (4, 'Sample Album 4', 4),
       (5, 'Sample Album 5', 5),
       (6, 'Sample Album 6', 6),
       (7, 'Sample Album 7', 7),
       (8, 'Sample Album 8', 8),
       (9, 'Sample Album 9', 9),
       (10, 'Sample Album 10', 10);

-- Menanamkan data dummy ke tabel Song
INSERT INTO "Songs" ("Id", "Title")
VALUES (1, 'Song 1'),
       (2, 'Song 2'),
       (3, 'Song 3'),
       (4, 'Song 4'),
       (5, 'Song 5'),
       (6, 'Song 6'),
       (7, 'Song 7'),
       (8, 'Song 8'),
       (9, 'Song 9'),
       (10, 'Song 10');

-- Menanamkan data dummy ke tabel AlbumSong (Many-to-Many Relationship)
INSERT INTO "AlbumSong" ("AlbumId", "SongId")
VALUES (1, 1),
       (1, 2),
       (2, 3),
       (2, 4),
       (3, 5),
       (3, 6),
       (4, 7),
       (4, 8),
       (5, 9),
       (5, 10);