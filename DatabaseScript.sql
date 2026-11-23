-- 1. Создание базы данных
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Shop_Shoe')
BEGIN
    CREATE DATABASE Shop_Shoe;
END
GO

USE Shop_Shoe;
GO

-- 2. Создание таблиц

CREATE TABLE dbo.roles (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL
);

CREATE TABLE dbo.kategories (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.subkategories (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.manufacturers (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    city NVARCHAR(100) NULL
);

CREATE TABLE dbo.sizes (
    id INT PRIMARY KEY IDENTITY(1,1),
    size DECIMAL(4,1) NOT NULL 
);

-- Основные таблицы
CREATE TABLE dbo.users (
    id INT PRIMARY KEY,
    roles_id INT NOT NULL,
    surname NVARCHAR(50) NOT NULL,
    name NVARCHAR(50) NOT NULL,
    patronymic NVARCHAR(50) NULL,
    login NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_users_roles FOREIGN KEY (roles_id) REFERENCES dbo.roles(id),
    CONSTRAINT UQ_users_login UNIQUE (login)
);

CREATE TABLE dbo.products (
    id INT PRIMARY KEY,
    categories_id INT NOT NULL,
    subcategories_id INT NOT NULL,
    manufacturers_id INT NULL, 
    picture NVARCHAR(100) NULL,
    name_product NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX) NULL,
    structure NVARCHAR(MAX) NULL,
    price DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_products_categories FOREIGN KEY (categories_id) REFERENCES dbo.kategories(id),
    CONSTRAINT FK_products_subcategories FOREIGN KEY (subcategories_id) REFERENCES dbo.subkategories(id),
    CONSTRAINT FK_products_manufacturers FOREIGN KEY (manufacturers_id) REFERENCES dbo.manufacturers(id)
);

CREATE TABLE dbo.stock_items (
    id INT PRIMARY KEY,
    products_id INT NOT NULL,
    sizes_id INT NOT NULL,
    quantity INT NOT NULL,
    CONSTRAINT UQ_stock_product_size UNIQUE (products_id, sizes_id),
    CONSTRAINT FK_stock_products FOREIGN KEY (products_id) REFERENCES dbo.products(id),
    CONSTRAINT FK_stock_sizes FOREIGN KEY (sizes_id) REFERENCES dbo.sizes(id)
);

CREATE TABLE dbo.orders (
    id INT PRIMARY KEY IDENTITY(1,1), 
    user_id INT NOT NULL,
    date_order DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_orders_users FOREIGN KEY (user_id) REFERENCES dbo.users(id)
);

CREATE TABLE dbo.orders_items (
    id INT PRIMARY KEY IDENTITY(1,1),
    orders_id INT NOT NULL,
    products_id INT NOT NULL,
    size_id INT NOT NULL,
    quantity INT NOT NULL,
    price_per_unit DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_oi_orders FOREIGN KEY (orders_id) REFERENCES dbo.orders(id),
    CONSTRAINT FK_oi_products FOREIGN KEY (products_id) REFERENCES dbo.products(id),
    CONSTRAINT FK_oi_sizes FOREIGN KEY (size_id) REFERENCES dbo.sizes(id)
);
GO

-- 3. Создание ролей базы данных и прав доступа

-- Создаем роли базы данных
CREATE ROLE db_manager;
CREATE ROLE db_admin;
CREATE ROLE db_client;

-- Выдаем права Администратору (все права на все таблицы)
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.orders TO db_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.orders_items TO db_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.products TO db_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.stock_items TO db_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.users TO db_admin;

-- Выдаем права Менеджеру
GRANT SELECT ON dbo.users TO db_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.orders TO db_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.orders_items TO db_manager;
GRANT SELECT ON dbo.products TO db_manager;
GRANT SELECT ON dbo.stock_items TO db_manager;

-- Выдаем права Клиенту
GRANT SELECT ON dbo.products TO db_client;
GRANT SELECT ON dbo.stock_items TO db_client;
GRANT SELECT ON dbo.sizes TO db_client;
GRANT INSERT ON dbo.orders TO db_client;
GRANT INSERT ON dbo.orders_items TO db_client;
GO

-- 4. Присваивание ролей пользователям

-- 1. Администратор (Иванов Иван)
UPDATE dbo.users 
SET roles_id = (SELECT id FROM dbo.roles WHERE name = 'Администратор') 
WHERE login = 'isivanov';

-- 2. Менеджер (Петров Пётр)
UPDATE dbo.users 
SET roles_id = (SELECT id FROM dbo.roles WHERE name = 'Менеджер') 
WHERE login = 'papetrov';

-- 3. Авторизованный пользователь (Сидорова Анна)
UPDATE dbo.users 
SET roles_id = (SELECT id FROM dbo.roles WHERE name = 'Авторизованный пользователь') 
WHERE login = 'asidorova';
GO