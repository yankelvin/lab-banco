CREATE TABLE LocadoraDW.DM_Socio
(
    ID_SOC INT IDENTITY NOT NULL,
    NOM_SOC VARCHAR(40) NOT NULL,
    TIPO_SOCIO VARCHAR(40) NOT NULL
)

ALTER TABLE LocadoraDW.DM_Socio
   ADD CONSTRAINT PK_DM_Socio_ID_SOC PRIMARY KEY CLUSTERED (ID_SOC);

ALTER TABLE LocadoraDW.FT_Locacoes
ADD CONSTRAINT ID_SOC
FOREIGN KEY (ID_SOC) REFERENCES LocadoraDW.DM_Socio(ID_SOC);
