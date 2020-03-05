CREATE TABLE Vendas.DIM_Parcelas
(
    COD_PARC INT NOT NULL,
    DAT_VENC DATETIME NOT NULL,
    VAL_PARC FLOAT(2) NOT NULL,
    PARC_PAGA CHAR(1) NOT NULL
)

ALTER TABLE Vendas.DIM_Parcelas
   ADD CONSTRAINT PK_DIM_Parcelas_COD_PARC PRIMARY KEY CLUSTERED (COD_PARC);

ALTER TABLE Vendas.FAT_Itens_Nota
ADD CONSTRAINT COD_PARC
FOREIGN KEY (COD_PARC) REFERENCES Vendas.DIM_Parcelas(COD_PARC);
