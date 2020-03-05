CREATE TABLE Vendas.DIM_Notas_Fiscais
(
    NUM_NOTA INT NOT NULL,
    VAL_NOTA FLOAT(2) NOT NULL,
    PER_ICMS FLOAT(1) NOT NULL,
    PER_IPI FLOAT(1) NOT NULL,
    PER_FRETE FLOAT(1) NOT NULL,
    VAL_TOTAL FLOAT(2) NOT NULL,
    DAT_NOTA DATETIME NOT NULL,
    DAT_VENC DATETIME NOT NULL,
    STA_NOTA CHAR(1) NOT NULL
)

ALTER TABLE Vendas.DIM_Notas_Fiscais
   ADD CONSTRAINT PK_DIM_Notas_Fiscais_NUM_NOTA PRIMARY KEY CLUSTERED (NUM_NOTA);

ALTER TABLE Vendas.FAT_Itens_Nota
ADD CONSTRAINT NUM_NOTA
FOREIGN KEY (NUM_NOTA) REFERENCES Vendas.DIM_Notas_Fiscais(NUM_NOTA);