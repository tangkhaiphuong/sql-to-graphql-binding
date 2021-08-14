create table Product
(
    Id             varchar(36)    default newid()      not null
        constraint Product_pk
            primary key nonclustered,
    CreatedDate    datetimeoffset default getutcdate() not null,
    ModifiedDate   datetimeoffset default NULL,
    DeletedDate    datetimeoffset default NULL,
    State          nvarchar(36),
    ProductName    varchar(40)                         not null,
    SupplierId     varchar(36)                         not null,
    CategoryId     varchar(36)                         not null,
    UnitPrice      decimal(13, 2)                      not null,
    IsDiscontinued bit                                 not null
)
go

INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'1', N'2021-08-11 11:29:36.3800000 +00:00', null, null, null, N'Product HHYDP', N'1', N'1', 18.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'2', N'2021-08-11 11:29:36.3866667 +00:00', null, null, null, N'Product RECZE', N'1', N'1', 19.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'3', N'2021-08-11 11:29:36.3933333 +00:00', null, null, null, N'Product IMEHJ', N'1', N'2', 10.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'4', N'2021-08-11 11:29:36.3966667 +00:00', null, null, null, N'Product KSBRM', N'2', N'2', 22.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'5', N'2021-08-11 11:29:36.4033333 +00:00', null, null, null, N'Product EPEIM', N'2', N'2', 21.35, 1);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'6', N'2021-08-11 11:29:36.4066667 +00:00', null, null, null, N'Product VAIIV', N'3', N'2', 25.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'7', N'2021-08-11 11:29:36.4200000 +00:00', null, null, null, N'Product HMLNI', N'3', N'7', 30.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'8', N'2021-08-11 11:29:36.4266667 +00:00', null, null, null, N'Product WVJFP', N'3', N'2', 40.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'9', N'2021-08-11 11:29:36.4300000 +00:00', null, null, null, N'Product AOZBW', N'4', N'6', 97.00, 1);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'10', N'2021-08-11 11:29:36.4366667 +00:00', null, null, null, N'Product YHXGE', N'4', N'8', 31.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'11', N'2021-08-11 11:29:36.4400000 +00:00', null, null, null, N'Product QMVUN', N'5', N'4', 21.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'12', N'2021-08-11 11:29:36.4466667 +00:00', null, null, null, N'Product OSFNS', N'5', N'4', 38.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'13', N'2021-08-11 11:29:36.4500000 +00:00', null, null, null, N'Product POXFU', N'6', N'8', 6.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'14', N'2021-08-11 11:29:36.4533333 +00:00', null, null, null, N'Product PWCJB', N'6', N'7', 23.25, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'15', N'2021-08-11 11:29:36.4600000 +00:00', null, null, null, N'Product KSZOI', N'6', N'2', 15.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'16', N'2021-08-11 11:29:36.4633333 +00:00', null, null, null, N'Product PAFRH', N'7', N'3', 17.45, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'17', N'2021-08-11 11:29:36.4666667 +00:00', null, null, null, N'Product BLCAX', N'7', N'6', 39.00, 1);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'18', N'2021-08-11 11:29:36.4700000 +00:00', null, null, null, N'Product CKEDC', N'7', N'8', 62.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'19', N'2021-08-11 11:29:36.4766667 +00:00', null, null, null, N'Product XKXDO', N'8', N'3', 9.20, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'20', N'2021-08-11 11:29:36.4800000 +00:00', null, null, null, N'Product QHFFP', N'8', N'3', 81.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'21', N'2021-08-11 11:29:36.4833333 +00:00', null, null, null, N'Product VJZZH', N'8', N'3', 10.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'22', N'2021-08-11 11:29:36.4900000 +00:00', null, null, null, N'Product CPHFY', N'9', N'5', 21.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'23', N'2021-08-11 11:29:36.4933333 +00:00', null, null, null, N'Product JLUDZ', N'9', N'5', 9.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'24', N'2021-08-11 11:29:36.5000000 +00:00', null, null, null, N'Product QOGNU', N'10', N'1', 4.50, 1);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'25', N'2021-08-11 11:29:36.5033333 +00:00', null, null, null, N'Product LYLNI', N'11', N'3', 14.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'26', N'2021-08-11 11:29:36.5266667 +00:00', null, null, null, N'Product HLGZA', N'11', N'3', 31.23, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'27', N'2021-08-11 11:29:36.5300000 +00:00', null, null, null, N'Product SMIOH', N'11', N'3', 43.90, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'28', N'2021-08-11 11:29:36.5366667 +00:00', null, null, null, N'Product OFBNT', N'12', N'7', 45.60, 1);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'29', N'2021-08-11 11:29:36.5400000 +00:00', null, null, null, N'Product VJXYN', N'12', N'6', 123.79, 1);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'30', N'2021-08-11 11:29:36.5433333 +00:00', null, null, null, N'Product LYERX', N'13', N'8', 25.89, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'31', N'2021-08-11 11:29:36.5466667 +00:00', null, null, null, N'Product XWOXC', N'14', N'4', 12.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'32', N'2021-08-11 11:29:36.5500000 +00:00', null, null, null, N'Product NUNAW', N'14', N'4', 32.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'33', N'2021-08-11 11:29:36.5566667 +00:00', null, null, null, N'Product ASTMN', N'15', N'4', 2.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'34', N'2021-08-11 11:29:36.5600000 +00:00', null, null, null, N'Product SWNJY', N'16', N'1', 14.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'35', N'2021-08-11 11:29:36.5633333 +00:00', null, null, null, N'Product NEVTJ', N'16', N'1', 18.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'36', N'2021-08-11 11:29:36.5666667 +00:00', null, null, null, N'Product GMKIJ', N'17', N'8', 19.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'37', N'2021-08-11 11:29:36.5700000 +00:00', null, null, null, N'Product EVFFA', N'17', N'8', 26.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'38', N'2021-08-11 11:29:36.5766667 +00:00', null, null, null, N'Product QDOMO', N'18', N'1', 263.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'39', N'2021-08-11 11:29:36.5833333 +00:00', null, null, null, N'Product LSOFL', N'18', N'1', 18.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'40', N'2021-08-11 11:29:36.5866667 +00:00', null, null, null, N'Product YZIXQ', N'19', N'8', 18.40, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'41', N'2021-08-11 11:29:36.5900000 +00:00', null, null, null, N'Product TTEEX', N'19', N'8', 9.65, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'42', N'2021-08-11 11:29:36.5966667 +00:00', null, null, null, N'Product RJVNM', N'20', N'5', 14.00, 1);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'43', N'2021-08-11 11:29:36.6000000 +00:00', null, null, null, N'Product ZZZHR', N'20', N'1', 46.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'44', N'2021-08-11 11:29:36.6033333 +00:00', null, null, null, N'Product VJIEO', N'20', N'2', 19.45, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'45', N'2021-08-11 11:29:36.6066667 +00:00', null, null, null, N'Product AQOKR', N'21', N'8', 9.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'46', N'2021-08-11 11:29:36.6133333 +00:00', null, null, null, N'Product CBRRL', N'21', N'8', 12.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'47', N'2021-08-11 11:29:36.6166667 +00:00', null, null, null, N'Product EZZPR', N'22', N'3', 9.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'48', N'2021-08-11 11:29:36.6200000 +00:00', null, null, null, N'Product MYNXN', N'22', N'3', 12.75, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'49', N'2021-08-11 11:29:36.6266667 +00:00', null, null, null, N'Product FPYPN', N'23', N'3', 20.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'50', N'2021-08-11 11:29:36.6300000 +00:00', null, null, null, N'Product BIUDV', N'23', N'3', 16.25, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'51', N'2021-08-11 11:29:36.6366667 +00:00', null, null, null, N'Product APITJ', N'24', N'7', 53.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'52', N'2021-08-11 11:29:36.6400000 +00:00', null, null, null, N'Product QSRXF', N'24', N'5', 7.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'53', N'2021-08-11 11:29:36.6466667 +00:00', null, null, null, N'Product BKGEA', N'24', N'6', 32.80, 1);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'54', N'2021-08-11 11:29:36.6500000 +00:00', null, null, null, N'Product QAQRL', N'25', N'6', 7.45, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'55', N'2021-08-11 11:29:36.6533333 +00:00', null, null, null, N'Product YYWRT', N'25', N'6', 24.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'56', N'2021-08-11 11:29:36.6666667 +00:00', null, null, null, N'Product VKCMF', N'26', N'5', 38.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'57', N'2021-08-11 11:29:36.6700000 +00:00', null, null, null, N'Product OVLQI', N'26', N'5', 19.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'58', N'2021-08-11 11:29:36.7100000 +00:00', null, null, null, N'Product ACRVI', N'27', N'8', 13.25, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'59', N'2021-08-11 11:29:36.7433333 +00:00', null, null, null, N'Product UKXRI', N'28', N'4', 55.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'60', N'2021-08-11 11:29:36.7466667 +00:00', null, null, null, N'Product WHBYK', N'28', N'4', 34.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'61', N'2021-08-11 11:29:36.7500000 +00:00', null, null, null, N'Product XYZPE', N'29', N'2', 28.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'62', N'2021-08-11 11:29:36.7566667 +00:00', null, null, null, N'Product WUXYK', N'29', N'3', 49.30, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'63', N'2021-08-11 11:29:36.7600000 +00:00', null, null, null, N'Product ICKNK', N'7', N'2', 43.90, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'64', N'2021-08-11 11:29:36.7633333 +00:00', null, null, null, N'Product HCQDE', N'12', N'5', 33.25, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'65', N'2021-08-11 11:29:36.7666667 +00:00', null, null, null, N'Product XYWBZ', N'2', N'2', 21.05, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'66', N'2021-08-11 11:29:36.7700000 +00:00', null, null, null, N'Product LQMGN', N'2', N'2', 17.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'67', N'2021-08-11 11:29:36.7766667 +00:00', null, null, null, N'Product XLXQF', N'16', N'1', 14.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'68', N'2021-08-11 11:29:36.7800000 +00:00', null, null, null, N'Product TBTBL', N'8', N'3', 12.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'69', N'2021-08-11 11:29:36.7866667 +00:00', null, null, null, N'Product COAXA', N'15', N'4', 36.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'70', N'2021-08-11 11:29:36.7933333 +00:00', null, null, null, N'Product TOONT', N'7', N'1', 15.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'71', N'2021-08-11 11:29:36.7966667 +00:00', null, null, null, N'Product MYMOI', N'15', N'4', 21.50, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'72', N'2021-08-11 11:29:36.8000000 +00:00', null, null, null, N'Product GEEOO', N'14', N'4', 34.80, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'73', N'2021-08-11 11:29:36.8033333 +00:00', null, null, null, N'Product WEUJZ', N'17', N'8', 15.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'74', N'2021-08-11 11:29:36.8100000 +00:00', null, null, null, N'Product BKAZJ', N'4', N'7', 10.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'75', N'2021-08-11 11:29:36.8100000 +00:00', null, null, null, N'Product BWRLG', N'12', N'1', 7.75, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'76', N'2021-08-11 11:29:36.8166667 +00:00', null, null, null, N'Product JYGFE', N'23', N'1', 18.00, 0);
INSERT INTO ProductShipping.dbo.Product (Id, CreatedDate, ModifiedDate, DeletedDate, State, ProductName, SupplierId, CategoryId, UnitPrice, IsDiscontinued) VALUES (N'77', N'2021-08-11 11:29:36.8200000 +00:00', null, null, null, N'Product LUNZZ', N'12', N'2', 13.00, 0);