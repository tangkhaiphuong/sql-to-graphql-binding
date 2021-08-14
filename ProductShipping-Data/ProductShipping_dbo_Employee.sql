create table Employee
(
    Id              varchar(36)    default newid()      not null
        constraint Employee_pk
            primary key nonclustered,
    CreatedDate     datetimeoffset default getutcdate() not null,
    ModifiedDate    datetimeoffset default NULL,
    DeletedDate     datetimeoffset default NULL,
    State           nvarchar(36),
    FirstName       nvarchar(255)                       not null,
    LastName        nvarchar(255)                       not null,
    Title           nvarchar(30)                        not null,
    TitleofCourtesy varchar(25)                         not null,
    BirthDate       datetime                            not null,
    HireDate        datetime                            not null,
    Address         nvarchar(60)                        not null,
    City            nvarchar(15)                        not null,
    Region          nvarchar(15),
    PostalCode      nvarchar(10),
    Country         nvarchar(15)                        not null,
    Phone           nvarchar(24)                        not null,
    ManagerId       varchar(36)
)
go

INSERT INTO ProductShipping.dbo.Employee (Id, CreatedDate, ModifiedDate, DeletedDate, State, FirstName, LastName, Title, TitleofCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, Phone, ManagerId) VALUES (N'1', N'2021-08-11 11:16:28.8466667 +00:00', null, null, null, N'Sara', N'Davis', N'CEO', N'Ms.', N'1958-12-08 00:00:00.000', N'2002-05-01 00:00:00.000', N'7890 - 20th Ave. E., Apt. 2A', N'Seattle', N'WA', N'10003', N'USA', N'(206) 555-0101', null);
INSERT INTO ProductShipping.dbo.Employee (Id, CreatedDate, ModifiedDate, DeletedDate, State, FirstName, LastName, Title, TitleofCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, Phone, ManagerId) VALUES (N'2', N'2021-08-11 11:16:28.8533333 +00:00', null, null, null, N'Don', N'Funk', N'Vice President, Sales', N'Dr.', N'1962-02-19 00:00:00.000', N'2002-08-14 00:00:00.000', N'9012 W. Capital Way', N'Tacoma', N'WA', N'10001', N'USA', N'(206) 555-0100', N'1');
INSERT INTO ProductShipping.dbo.Employee (Id, CreatedDate, ModifiedDate, DeletedDate, State, FirstName, LastName, Title, TitleofCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, Phone, ManagerId) VALUES (N'3', N'2021-08-11 11:16:28.8566667 +00:00', null, null, null, N'Judy', N'Lew', N'Sales Manager', N'Ms.', N'1973-08-30 00:00:00.000', N'2002-04-01 00:00:00.000', N'2345 Moss Bay Blvd.', N'Kirkland', N'WA', N'10007', N'USA', N'(206) 555-0103', N'2');
INSERT INTO ProductShipping.dbo.Employee (Id, CreatedDate, ModifiedDate, DeletedDate, State, FirstName, LastName, Title, TitleofCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, Phone, ManagerId) VALUES (N'4', N'2021-08-11 11:16:28.8666667 +00:00', null, null, null, N'Yael', N'Peled', N'Sales Representative', N'Mrs.', N'1947-09-19 00:00:00.000', N'2003-05-03 00:00:00.000', N'5678 Old Redmond Rd.', N'Redmond', N'WA', N'10009', N'USA', N'(206) 555-0104', N'3');
INSERT INTO ProductShipping.dbo.Employee (Id, CreatedDate, ModifiedDate, DeletedDate, State, FirstName, LastName, Title, TitleofCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, Phone, ManagerId) VALUES (N'5', N'2021-08-11 11:16:28.8700000 +00:00', null, null, null, N'Sven', N'Buck', N'Sales Manager', N'Mr.', N'1965-03-04 00:00:00.000', N'2003-10-17 00:00:00.000', N'8901 Garrett Hill', N'London', null, N'10004', N'UK', N'(71) 234-5678', N'2');
INSERT INTO ProductShipping.dbo.Employee (Id, CreatedDate, ModifiedDate, DeletedDate, State, FirstName, LastName, Title, TitleofCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, Phone, ManagerId) VALUES (N'6', N'2021-08-11 11:16:28.8800000 +00:00', null, null, null, N'Paul', N'Suurs', N'Sales Representative', N'Mr.', N'1973-07-02 00:00:00.000', N'2003-10-17 00:00:00.000', N'3456 Coventry House, Miner Rd.', N'London', null, N'10005', N'UK', N'(71) 345-6789', N'5');
INSERT INTO ProductShipping.dbo.Employee (Id, CreatedDate, ModifiedDate, DeletedDate, State, FirstName, LastName, Title, TitleofCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, Phone, ManagerId) VALUES (N'7', N'2021-08-11 11:16:28.8833333 +00:00', null, null, null, N'Russell', N'King', N'Sales Representative', N'Mr.', N'1970-05-29 00:00:00.000', N'2004-01-02 00:00:00.000', N'6789 Edgeham Hollow, Winchester Way', N'London', null, N'10002', N'UK', N'(71) 123-4567', N'5');
INSERT INTO ProductShipping.dbo.Employee (Id, CreatedDate, ModifiedDate, DeletedDate, State, FirstName, LastName, Title, TitleofCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, Phone, ManagerId) VALUES (N'8', N'2021-08-11 11:16:28.8866667 +00:00', null, null, null, N'Maria', N'Cameron', N'Sales Representative', N'Ms.', N'1968-01-09 00:00:00.000', N'2004-03-05 00:00:00.000', N'4567 - 11th Ave. N.E.', N'Seattle', N'WA', N'10006', N'USA', N'(206) 555-0102', N'3');
INSERT INTO ProductShipping.dbo.Employee (Id, CreatedDate, ModifiedDate, DeletedDate, State, FirstName, LastName, Title, TitleofCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, Phone, ManagerId) VALUES (N'9', N'2021-08-11 11:16:28.8900000 +00:00', null, null, null, N'Zoya', N'Dolgopyatova', N'Sales Representative', N'Ms.', N'1976-01-27 00:00:00.000', N'2004-11-15 00:00:00.000', N'1234 Houndstooth Rd.', N'London', null, N'10008', N'UK', N'(71) 456-7890', N'5');