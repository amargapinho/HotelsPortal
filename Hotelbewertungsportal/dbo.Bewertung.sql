CREATE TABLE [dbo].[Bewertung] (
    [Id]         INT          IDENTITY (1, 1) NOT NULL,
    [Username]   VARCHAR (50) NULL,
    [Hotelname]  VARCHAR (50) NULL,
    [Datum]      DATE         NULL,
    [Dauer]      VARCHAR (50) NULL,
    [Bewertung]  INT          NULL,
    [Empfehlung] BIT          NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

