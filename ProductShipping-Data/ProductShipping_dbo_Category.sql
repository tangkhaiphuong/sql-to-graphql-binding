create table Category
(
    Id           varchar(36)    default newid()      not null
        constraint Category_pk
            primary key nonclustered,
    CreatedDate  datetimeoffset default getutcdate() not null,
    ModifiedDate datetimeoffset default NULL,
    DeletedDate  datetimeoffset default NULL,
    State        nvarchar(36),
    CategoryName nvarchar(15)                        not null,
    Description  nvarchar(200)                       not null
)
go

INSERT INTO ProductShipping.dbo.Category (Id, CreatedDate, ModifiedDate, DeletedDate, State, CategoryName, Description) VALUES (N'1', N'2021-08-11 11:29:04.4400000 +00:00', null, null, null, N'Beverages', N'Soft drinks, coffees, teas, beers, and ales');
INSERT INTO ProductShipping.dbo.Category (Id, CreatedDate, ModifiedDate, DeletedDate, State, CategoryName, Description) VALUES (N'2', N'2021-08-11 11:29:04.4533333 +00:00', null, null, null, N'Condiments', N'Sweet and savory sauces, relishes, spreads, and seasonings');
INSERT INTO ProductShipping.dbo.Category (Id, CreatedDate, ModifiedDate, DeletedDate, State, CategoryName, Description) VALUES (N'3', N'2021-08-11 11:29:04.4600000 +00:00', null, null, null, N'Confections', N'Desserts, candies, and sweet breads');
INSERT INTO ProductShipping.dbo.Category (Id, CreatedDate, ModifiedDate, DeletedDate, State, CategoryName, Description) VALUES (N'4', N'2021-08-11 11:29:04.4633333 +00:00', null, null, null, N'Dairy Products', N'Cheeses');
INSERT INTO ProductShipping.dbo.Category (Id, CreatedDate, ModifiedDate, DeletedDate, State, CategoryName, Description) VALUES (N'5', N'2021-08-11 11:29:04.4666667 +00:00', null, null, null, N'Grains/Cereals', N'Breads, crackers, pasta, and cereal');
INSERT INTO ProductShipping.dbo.Category (Id, CreatedDate, ModifiedDate, DeletedDate, State, CategoryName, Description) VALUES (N'6', N'2021-08-11 11:29:04.4700000 +00:00', null, null, null, N'Meat/Poultry', N'Prepared meats');
INSERT INTO ProductShipping.dbo.Category (Id, CreatedDate, ModifiedDate, DeletedDate, State, CategoryName, Description) VALUES (N'7', N'2021-08-11 11:29:04.4766667 +00:00', null, null, null, N'Produce', N'Dried fruit and bean curd');
INSERT INTO ProductShipping.dbo.Category (Id, CreatedDate, ModifiedDate, DeletedDate, State, CategoryName, Description) VALUES (N'8', N'2021-08-11 11:29:04.4800000 +00:00', null, null, null, N'Seafood', N'Seaweed and fish');