CREATE DATABASE DB_Orders
GO
USE DB_Orders
GO

CREATE TABLE USERS(
	  Id INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL
);

CREATE TABLE ORDERS_HISTORY (
ID INT IDENTITY(1,1) PRIMARY KEY,
TX_NUMBER INT NOT NULL,
CHANGED_DATE DATETIME DEFAULT GETDATE(),
DESCRIPTION NVARCHAR(300) NOT NULL,
ACTION VARCHAR(4) NOT NULL,
STATUS_BEFORE VARCHAR(10)NOT NULL,
STATUS_AFTER VARCHAR(10) NULL,
ID_SYMBOL INT NOT NULL
);

ALTER TABLE ORDERS_HISTORY
ADD CONSTRAINT check_types_1
CHECK (ACTION IN ('SELL', 'BUY'));

CREATE TABLE STOCK_MARKET_SHARES (
ID INT IDENTITY(1,1) PRIMARY KEY,
SYMBOL VARCHAR(5) UNIQUE NOT NULL,
UNIT_PRICE DECIMAL(10, 2) NOT NULL
);

CREATE TABLE ORDERS (
TX_NUMBER INT IDENTITY(1,1) PRIMARY KEY,
ORDER_DATE DATETIME DEFAULT GETDATE(),
ACTION VARCHAR(4) NOT NULL,
STATUS VARCHAR(10) NOT NULL,
ID_SYMBOL INT NOT NULL,
QUANTITY INT NOT NULL,
FOREIGN KEY (ID_SYMBOL) REFERENCES STOCK_MARKET_SHARES(ID)
);
ALTER TABLE ORDERS
ADD CONSTRAINT check_types_2
CHECK (ACTION IN ('SELL', 'BUY'));


-- INSERT STOCK_MARKET_SHARES
INSERT INTO STOCK_MARKET_SHARES (SYMBOL, UNIT_PRICE) VALUES ('AAPL',
150.50);
INSERT INTO STOCK_MARKET_SHARES (SYMBOL, UNIT_PRICE) VALUES ('AMZN',
3450.25);
INSERT INTO STOCK_MARKET_SHARES (SYMBOL, UNIT_PRICE) VALUES ('GOOGL',
2700.75);
INSERT INTO STOCK_MARKET_SHARES (SYMBOL, UNIT_PRICE) VALUES ('TSLA',
780.80);
INSERT INTO STOCK_MARKET_SHARES (SYMBOL, UNIT_PRICE) VALUES ('MSFT',
305.40);

--INSERT ORDERS
INSERT INTO ORDERS (ACTION, STATUS, ID_SYMBOL, QUANTITY)
VALUES ('BUY', 'PENDING', 1, 150);
INSERT INTO ORDERS (ACTION, STATUS, ID_SYMBOL, QUANTITY)
VALUES ('SELL', 'EXECUTED', 2, 233);
INSERT INTO ORDERS (ACTION, STATUS, ID_SYMBOL, QUANTITY)
VALUES ('BUY', 'PENDING', 3, 11);

--INSERT ORDERS HISTORY
INSERT INTO ORDERS_HISTORY (TX_NUMBER, DESCRIPTION, ACTION, STATUS_BEFORE, ID_SYMBOL)
VALUES (1,'ACCION COMRADA....', 'BUY', 'PENDING', 1);
INSERT INTO ORDERS_HISTORY (TX_NUMBER, DESCRIPTION, ACTION, STATUS_BEFORE, ID_SYMBOL)
VALUES (2,'ACCION VENDIDA....', 'SELL', 'EXECUTED', 2);


--STORE PROCEDURES
---------------------------------------------------------------
-- LISTAR ORDENES
CREATE PROCEDURE SP_ORDER_LIST
AS
BEGIN
    SELECT O.TX_NUMBER, O.ORDER_DATE, O.ACTION, O.STATUS, S.SYMBOL AS [SYMBOL], O.QUANTITY
    FROM ORDERS O
	INNER JOIN STOCK_MARKET_SHARES S ON O.ID_SYMBOL = S.ID
END;
-- LISTAR ORDENES POR PK
CREATE PROCEDURE SP_ORDER_LIST_TX
	@TX_NUMBER INT
AS
BEGIN
    SELECT O.TX_NUMBER, O.ORDER_DATE, O.ACTION, O.STATUS, S.SYMBOL AS [SYMBOL], O.QUANTITY
    FROM ORDERS O
	INNER JOIN STOCK_MARKET_SHARES S ON O.ID_SYMBOL = S.ID
	WHERE(@TX_NUMBER = O.TX_NUMBER)
END;
-- CREAR ORDENES
CREATE PROCEDURE SP_ORDER_CREATE
    @ACTION VARCHAR(4),
    @STATUS VARCHAR(10),
    @ID_SYMBOL INT,
	@QUANTITY INT
AS
BEGIN
	DECLARE @ORDER_DATE DATETIME = GETDATE();
    INSERT INTO ORDERS(ORDER_DATE, ACTION, STATUS, ID_SYMBOL, QUANTITY)
    VALUES (@ORDER_DATE, @ACTION, @STATUS, @ID_SYMBOL, @QUANTITY);

    -- ID de la orden creada
    DECLARE @NewOrderId INT = CAST(SCOPE_IDENTITY() AS INT);
	    -- Devolver todos los detalles de la nueva orden
    SELECT 
        @NewOrderId AS TX_NUMBER,
		@ORDER_DATE AS ORDER_DATE,
        @ACTION AS ACTION,
        @STATUS AS STATUS,
        @ID_SYMBOL AS ID_SYMBOL,
        @QUANTITY AS QUANTITY;
	-- Llamar al procedimiento para crear un registro en el historial
    EXEC SP_ORDER_CREATED @NewOrderId, @ACTION, @STATUS, @ID_SYMBOL;
END;
-- ELIMINAR ORDENES
CREATE PROCEDURE SP_ORDER_DELETE
    @TX_NUMBER INT
AS
BEGIN
        DELETE FROM ORDERS
        WHERE TX_NUMBER = @TX_NUMBER;
END;


------------------------------------------------------
-- LISTAR STOCK_MARKED_SHARED
CREATE PROCEDURE SP_STOCK_LIST
AS
BEGIN
    SELECT ID, SYMBOL, UNIT_PRICE
    FROM STOCK_MARKET_SHARES
END;
-- LISTAR STOCK POR PK
CREATE PROCEDURE SP_STOCK_LIST_PK
	@ID INT
AS
BEGIN
    SELECT ID, SYMBOL, UNIT_PRICE
    FROM STOCK_MARKET_SHARES
	WHERE(ID = @ID)
END;
-- CREAR STOCK_MARKED_SHARED
CREATE PROCEDURE SP_STOCK_CREATE
	@SYMBOL VARCHAR(5),
	@UNIT_PRICE DECIMAL(10, 2)
AS
BEGIN
    INSERT INTO STOCK_MARKET_SHARES(SYMBOL, UNIT_PRICE)
    VALUES (@SYMBOL, @UNIT_PRICE);

    DECLARE @NewStockId INT = CAST(SCOPE_IDENTITY() AS INT);
	    -- Devolver todos los detalles
    SELECT 
        @NewStockId AS ID,
        @SYMBOL AS SYMBOL,
        @UNIT_PRICE AS UNIT_PRICE;
END;
-- ELIMINAR STOCK
CREATE PROCEDURE SP_STOCK_DELETE
    @ID INT
AS
BEGIN
        DELETE FROM STOCK_MARKET_SHARES
        WHERE ID = @ID;
END;

------------------------------------------------------------------
-- LISTAR ORDERS_HISTORY
CREATE PROCEDURE SP_OH_LIST
AS
BEGIN
    SELECT OH.ID, OH.TX_NUMBER, OH.CHANGED_DATE, OH.DESCRIPTION, OH.ACTION, OH.STATUS_BEFORE, OH.STATUS_AFTER, S.SYMBOL
    FROM ORDERS_HISTORY OH
	INNER JOIN STOCK_MARKET_SHARES S ON OH.ID_SYMBOL = S.ID
END;


-------------------------------------------------------------------------
-- CREAR UN HISTORIAL EN ORDERS_HISTORY CUANDO SE CREE UNA NUEVA ORDEN
CREATE PROCEDURE SP_ORDER_CREATED
    @TX_NUMBER INT,
    @ACTION VARCHAR(4),
    @STATUS VARCHAR(10),
    @ID_SYMBOL INT
AS
BEGIN
    DECLARE @Description NVARCHAR(300);
	DECLARE @Symbol NVARCHAR(5);

    -- Obtener el nombre del s�mbolo desde la tabla SYMBOL
    SELECT @Symbol = SYMBOL
    FROM STOCK_MARKET_SHARES
    WHERE ID = @ID_SYMBOL;

    -- Construir la descripci�n detallada con informaci�n relevante de los campos
    SET @Description = CONCAT(
        'Orden creada con TX_NUMBER: ', @TX_NUMBER, 
        ', Acci�n: ', @ACTION, 
        ', Estado inicial: ', @STATUS, 
        ', S�mbolo: ', @Symbol
    );

    -- Insertar un nuevo registro en la tabla de historial con la descripci�n construida
    INSERT INTO ORDERS_HISTORY (TX_NUMBER, DESCRIPTION, ACTION, STATUS_BEFORE, ID_SYMBOL)
    VALUES (@TX_NUMBER, @Description, @ACTION, @STATUS, @ID_SYMBOL);
END;


--------------------------------------------------------------------------------------
-- CREAR UN HISTORIAL EN ORDERS_HISTORY CUANDO SE MODIFIQUE EL ESTADO DE UNA ORDEN
CREATE PROCEDURE SP_ORDERS_CHANGED
    @TX_NUMBER INT,
    @STATUS VARCHAR(10)
AS
BEGIN

    -- Obtener el estado actual del registro en la tabla principal antes de la modificaci�n
    DECLARE @STATUS_BEFORE VARCHAR(10);

	DECLARE @Description NVARCHAR(300);
	DECLARE @Symbol NVARCHAR(5);
	DECLARE @ACTION VARCHAR(4);
	DECLARE @ID_SYMBOL INT;
	DECLARE @ORDER_DATE DATETIME = GETDATE();
	DECLARE @QUANTITY INT;

    SELECT @STATUS_BEFORE = STATUS FROM ORDERS WHERE TX_NUMBER = @TX_NUMBER;
	SELECT @ACTION = ACTION FROM ORDERS WHERE TX_NUMBER = @TX_NUMBER;
	SELECT @ID_SYMBOL = ID_SYMBOL FROM ORDERS WHERE TX_NUMBER = @TX_NUMBER;
	SELECT @QUANTITY = QUANTITY FROM ORDERS WHERE TX_NUMBER = @TX_NUMBER;

    -- Obtener el nombre del s�mbolo desde la tabla SYMBOL
    SELECT @Symbol = SYMBOL
    FROM STOCK_MARKET_SHARES
    WHERE ID = @ID_SYMBOL;

    -- Construir la descripci�n detallada con informaci�n relevante de los campos
    SET @Description = CONCAT(
        'Orden modificada con TX_NUMBER: ', @TX_NUMBER, 
		', Fecha modificaci�n: ', @ORDER_DATE, 
        ', Acci�n: ', @ACTION, 
        ', Estado anterior: ', @STATUS_BEFORE, 
		', Estado actual: ', @STATUS, 
        ', S�mbolo: ', @Symbol
    );
    
    -- Insertar el registro actual en la tabla de historial
    INSERT INTO ORDERS_HISTORY(TX_NUMBER, CHANGED_DATE, DESCRIPTION, ACTION, STATUS_BEFORE, STATUS_AFTER, ID_SYMBOL)
    VALUES (@TX_NUMBER, @ORDER_DATE, @DESCRIPTION, @ACTION, @STATUS_BEFORE, @STATUS, @ID_SYMBOL);
    
    -- Realizar la modificaci�n en la tabla principal
    UPDATE ORDERS
    SET STATUS = @STATUS
    WHERE TX_NUMBER = @TX_NUMBER;

	SELECT 
        @TX_NUMBER AS TX_NUMBER,
		@ORDER_DATE AS ORDER_DATE,
        @ACTION AS ACTION,
        @STATUS AS STATUS,
        @ID_SYMBOL AS ID_SYMBOL,
        @QUANTITY AS QUANTITY;
END;


CREATE PROCEDURE SP_LIST_ORDERS_EXECUTED
AS
BEGIN
    SELECT O.TX_NUMBER, O.ORDER_DATE, O.ACTION, O.STATUS, S.SYMBOL AS [SYMBOL], O.QUANTITY, CAST(O.QUANTITY * S.UNIT_PRICE AS DECIMAL(10, 2)) AS [MONTO_NETO]
    FROM ORDERS O
	INNER JOIN STOCK_MARKET_SHARES S ON O.ID_SYMBOL = S.ID
	WHERE O.STATUS = 'EXECUTED';
END;

CREATE PROCEDURE SP_LIST_ORDERS_BY_YEAR
	@YEAR INT
AS
BEGIN
    SELECT O.TX_NUMBER, O.ORDER_DATE, O.ACTION, O.STATUS, S.SYMBOL AS [SYMBOL], O.QUANTITY
    FROM ORDERS O
	INNER JOIN STOCK_MARKET_SHARES S ON O.ID_SYMBOL = S.ID
	WHERE YEAR(O.ORDER_DATE) = @YEAR;
END;

CREATE PROCEDURE SP_ORDER_BUY_OR_SELL_PENDING
    @ACTION VARCHAR(4),
    @ID_SYMBOL INT,
	@QUANTITY INT
AS
BEGIN
	DECLARE @ORDER_DATE DATETIME = GETDATE();
	DECLARE @STATUS VARCHAR(10) = 'EXECUTED';
    INSERT INTO ORDERS(ORDER_DATE, ACTION, STATUS, ID_SYMBOL, QUANTITY)
    VALUES (@ORDER_DATE, @ACTION, @STATUS, @ID_SYMBOL, @QUANTITY);

    -- ID de la orden creada
    DECLARE @NewOrderId INT = CAST(SCOPE_IDENTITY() AS INT);
	    -- Devolver todos los detalles de la nueva orden
    SELECT 
        @NewOrderId AS TX_NUMBER,
		@ORDER_DATE AS ORDER_DATE,
        @ACTION AS ACTION,
        @STATUS AS STATUS,
        @ID_SYMBOL AS ID_SYMBOL,
        @QUANTITY AS QUANTITY;
	-- Llamar al procedimiento para crear un registro en el historial
    EXEC SP_ORDER_CREATED @NewOrderId, @ACTION, @STATUS, @ID_SYMBOL;
END;

SELECT * FROM ORDERS;

SELECT * FROM STOCK_MARKET_SHARES;

SELECT * FROM ORDERS_HISTORY;

SELECT * FROM USERS;







