﻿CREATE TABLE [dbo].[Candidates] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]           NVARCHAR (50)  NOT NULL,
    [LastName]            NVARCHAR (50)  NOT NULL,
    [IsEntollment]        BIT            NOT NULL,
    [Address]             NVARCHAR (70)  NULL,
    [BirthCity]           NVARCHAR (70)  NULL,
    [BirthCountry]        NVARCHAR (70)  NULL,
    [BirthState]          NVARCHAR (2)   NULL,
    [City]                NVARCHAR (70)  NULL,
    [DOB]                 NVARCHAR (MAX) NULL,
    [DriverLicenseNumber] NVARCHAR (35)  NULL,
    [DriverLicenseState]  NVARCHAR (2)   NULL,
    [Email]               NVARCHAR (254) NULL,
    [Gender]              NVARCHAR (35)  NULL,
    [LicenseNumber]       NVARCHAR (35)  NULL,
    [MiddleName]          NVARCHAR (35)  NULL,
    [Photo]               NVARCHAR (MAX) NULL,
    [PostalCode]          NVARCHAR (35)  NULL,
    [PrimaryPhone]        NVARCHAR (35)  NULL,
    [SSN]                 NVARCHAR (MAX) NULL,
    [SecondaryPhone]      NVARCHAR (35)  NULL,
    [State]               NVARCHAR (2)   NULL,
    [Age]                 INT   NULL,
    CONSTRAINT [PK_Candidates] PRIMARY KEY CLUSTERED ([Id] ASC)
);

