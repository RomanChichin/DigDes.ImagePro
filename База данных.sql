use DB
USE DB_TEST


CREATE TABLE Users
(
UserID uniqueidentifier NOT NULL PRIMARY KEY,
Nickname varchar(25) UNIQUE NOT NULL,
RegistrationDate Date NOT NULL,
Email varchar(50) UNIQUE NOT NULL,
About varchar(200) NULL
)


CREATE TABLE Subscribers
(
PublisherID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Users(UserID),
SubscriberID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Users(UserID),
CONSTRAINT Impossible_to_subscribe_more_the_one_time UNIQUE (PublisherID,SubscriberID)
)



CREATE TABLE Posts
(
PostID uniqueidentifier NOT NULL PRIMARY KEY,
UserID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Users(UserID),
Photo varbinary(MAX) NOT NULL,
DateOfPublication DATETIME NOT NULL
)

CREATE TABLE Comments
(
CommentID uniqueidentifier NOT NULL PRIMARY KEY,
PostID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Posts(PostID) ON DELETE CASCADE,
UserID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Users(UserID), 
DateOfPublication DATETIME NOT NULL,
Comment varchar(300) NOT NULL
)

CREATE TABLE Likes
(
PostID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Posts(PostID) ON DELETE CASCADE,
UserID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Users(UserID),
CONSTRAINT Only_one_Like_by_user UNIQUE (PostID,UserID)
)

CREATE TABLE HashTags
(
HashTagID uniqueidentifier NOT NULL PRIMARY KEY,
HashTagText VARCHAR(30) NOT NULL
)

CREATE TABLE HashTagsToPosts
(
HashTagID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES HashTags(HashTagID),
PostID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Posts(PostID) ON DELETE CASCADE
)


CREATE PROCEDURE ClearDatabase
AS
TRUNCATE TABLE HashTagsToPosts
DELETE HashTags
TRUNCATE TABLE Likes
TRUNCATE TABLE Subscribers
TRUNCATE TABLE Comments
DELETE Posts
DELETE Users
GO







////////////////////////////////////////////

SELECT UserID,Nickname,RegistrationDate,Email,About FROM Users 
INNER JOIN Subscribers 
ON SubscriberID = UserID
WHERE PublisherID='6592183F-CAB4-4C26-8324-33D83E414AF1'

SELECT * FROM Users
DELETE FROM Users

UPDATE Users
SET 
Email='asfasfasff',About = 'asfasfasf'
WHERE Nickname='Bennnn'

DELETE FROM Users
WHERE Nickname='Ben' AND About='asfasfasf' 





////////////////////////////////////////////////////////////








SELECT HashTagText FROM HashTagsToPosts hp
INNER JOIN HashTags hs
ON hp.HashTagID=hs.HashTagID
WHERE hp.PostID='6592183F-CAB4-4C26-8324-33D83E414AF1'




CREATE TABLE HT
(
HashTagID int IDENTITY NOT NULL PRIMARY KEY,
HashTagText VARCHAR(30) NOT NULL
)
INSERT INTO HT
VALUES
('ht1'),('ht2'),('ht3'),('ht4'),('ht5')
SELECT * FROM HT

CREATE TABLE HTP
(
HashTagID int NOT NULL FOREIGN KEY REFERENCES HT(HashTagID),
PostID int NOT NULL FOREIGN KEY REFERENCES P(PostID) 
)
INSERT INTO HTP
VALUES
(1,1),(2,1),(3,1),(4,2),(4,2)
Select * from HTP

Create table P
(
PostID int IDENTITY,
Photo varchar(10) 
)


SELECT HashTagText FROM HT ht
INNER JOIN HTP hp
ON ht.HashTagID=hp.HashTagID
WHERE PostID = 1 

Select * from P

