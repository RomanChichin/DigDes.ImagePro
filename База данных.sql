use DB


CREATE TABLE Users
(
UserID uniqueidentifier NOT NULL PRIMARY KEY,
Nickname varchar(25) NOT NULL,
RegistrationDate Date NOT NULL,
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








