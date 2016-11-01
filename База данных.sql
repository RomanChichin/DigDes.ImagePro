use DB


CREATE TABLE Users
(
UserID uniqueidentifier NOT NULL PRIMARY KEY,
Nickname varchar(25) UNIQUE NOT NULL,
RegistrationDate Date NOT NULL,
)
/////////////////////
ALTER TABLE Users
ADD CONSTRAINT unik UNIQUE (Nickname);
/////////////////////////
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
UserID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Users(UserID)
) 

CREATE TABLE HashTags
(
HashTagID uniqueidentifier NOT NULL PRIMARY KEY,
Name VARCHAR(20)
)

CREATE TABLE HashTagsToPosts
(
HashTagID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES HashTags(HashTagID),
PostID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Posts(PostID) ON DELETE CASCADE
)

CREATE TABLE UserComments
(
UserID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Users(UserID),
CommentID uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Comments(CommentID) ON DELETE CASCADE
) 

SELECT * FROM Users
DELETE FROM Users

SELECT * FROM Comments








