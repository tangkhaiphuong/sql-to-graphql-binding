create table Shipper
(
    Id           varchar(36)    default newid()      not null
        constraint Shipper_pk
            primary key nonclustered,
    CreatedDate  datetimeoffset default getutcdate() not null,
    ModifiedDate datetimeoffset default NULL,
    DeletedDate  datetimeoffset default NULL,
    State        nvarchar(36),
    CompanyName  nvarchar(40)                        not null,
    Phone        nvarchar(30)                        not null
)
go

INSERT INTO ProductShipping.dbo.Shipper (Id, CreatedDate, ModifiedDate, DeletedDate, State, CompanyName, Phone) VALUES (N'1', N'2021-08-11 11:21:25.5366667 +00:00', null, null, null, N'Shipper GVSUA', N'(503) 555-0137');
INSERT INTO ProductShipping.dbo.Shipper (Id, CreatedDate, ModifiedDate, DeletedDate, State, CompanyName, Phone) VALUES (N'2', N'2021-08-11 11:21:25.5433333 +00:00', null, null, null, N'Shipper ETYNR', N'(425) 555-0136');
INSERT INTO ProductShipping.dbo.Shipper (Id, CreatedDate, ModifiedDate, DeletedDate, State, CompanyName, Phone) VALUES (N'3', N'2021-08-11 11:21:25.5466667 +00:00', null, null, null, N'Shipper ZHISN', N'(415) 555-0138');