CREATE TABLE LocadoraDW.DM_Titulo
(
    ID_TITULO INT IDENTITY NOT NULL,
    TPO_TITULO VARCHAR(40) NOT NULL,
    CLA_TITULO VARCHAR(40) NOT NULL,
    DSC_TITULO VARCHAR(40) NOT NULL
)

ALTER TABLE LocadoraDW.DM_Titulo
   ADD CONSTRAINT PK_DM_Titulo_ID_TITULO PRIMARY KEY CLUSTERED (ID_TITULO);

ALTER TABLE LocadoraDW.FT_Locacoes
ADD CONSTRAINT ID_TITULO
FOREIGN KEY (ID_TITULO) REFERENCES LocadoraDW.DM_Titulo(ID_TITULO);