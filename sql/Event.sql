CREATE TABLE Event
(
    ID        INT PRIMARY KEY IDENTITY(1,1),
    EventTask NVARCHAR(100),
    EventData NVARCHAR( MAX)
);