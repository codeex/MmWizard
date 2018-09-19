CREATE TABLE article (
  Id int(11) NOT NULL AUTO_INCREMENT,
  Sort int(11) DEFAULT 0,
  Title varchar(255) NOT NULL,
  Author varchar(255) DEFAULT NULL,
  KeyWords varchar(255) DEFAULT NULL,
  Content text DEFAULT NULL,
  UpdatedDate datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  TemplateName varchar(255) NOT NULL,
  PRIMARY KEY (Id),
  UNIQUE INDEX Id (Id),
  INDEX IDX_artical_UpdatedDate (UpdatedDate)
);

CREATE TABLE bigclass (
  ID int(11) NOT NULL AUTO_INCREMENT,
  Sort int(11) DEFAULT 0,
  Title varchar(255) DEFAULT NULL,
  PRIMARY KEY (ID),
  UNIQUE INDEX ID (ID)
);

CREATE TABLE businesschildren (
  Id int(11) NOT NULL AUTO_INCREMENT,
  sort int(11) DEFAULT 0,
  Title varchar(255) DEFAULT NULL,
  ParentId int(11) DEFAULT 0,
  PRIMARY KEY (Id),
  UNIQUE INDEX Id (Id)
);

CREATE TABLE bussiness (
  Id int(11) NOT NULL AUTO_INCREMENT,
  Sort int(11) DEFAULT 0,
  Title varchar(255) DEFAULT NULL,
  PRIMARY KEY (Id),
  UNIQUE INDEX Id (Id)
);

CREATE TABLE metainfo (
  Id int(11) NOT NULL AUTO_INCREMENT,
  KeyReleatedid int(11) NOT NULL DEFAULT 0,
  `Key` varchar(255) NOT NULL,
  Value varchar(255) DEFAULT NULL,
  KeyType int(11) DEFAULT 0,
  PRIMARY KEY (Id),
  UNIQUE INDEX Id (Id),
  INDEX IDX_metainfo_KeyReleatedid (KeyReleatedid)
);

CREATE TABLE pagetemplate (
  Id int(11) NOT NULL AUTO_INCREMENT,
  Name varchar(50) NOT NULL,
  Content text DEFAULT NULL,
  Class int(11) NOT NULL DEFAULT 0,
  UpdatedDate datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (Id),
  UNIQUE INDEX Id (Id),
  INDEX IDX_pagetemplate_UpdatedDate (UpdatedDate),
  UNIQUE INDEX UK_pagetemplate_Name (Name)
);
