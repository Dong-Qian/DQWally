CREATE DATABASE DQWally;

USE DQWally;
grant all privileges on dqwally.* to 'superuser'@'%' identified by 'Conestoga1';

CREATE TABLE customer (CustomerID BIGINT PRIMARY KEY AUTO_INCREMENT, FirstName VARCHAR(45),
					   LastName VARCHAR(45), PhoneNumber BIGINT);
			
CREATE TABLE branch (BranchID BIGINT PRIMARY KEY AUTO_INCREMENT, BranchName VARCHAR(45));

CREATE TABLE product (ProductID BIGINT PRIMARY KEY AUTO_INCREMENT, ProductName VARCHAR(45), 
					  Price DOUBLE, InventoryQuantity INT);
                      
CREATE TABLE orders (OrdersID BIGINT PRIMARY KEY AUTO_INCREMENT, OderDate DATETIME, ProductID BIGINT, 
					 CustomerID BIGINT, BranchID BIGINT, OrderQuantity INT, OrderStatus VARCHAR(10),
					 FOREIGN KEY (ProductID) REFERENCES product(ProductID),
                     FOREIGN KEY (CustomerID) REFERENCES customer(CustomerID),
					 FOREIGN KEY (BranchID) REFERENCES branch(BranchID));

CREATE TABLE orderLine (OrderLineID BIGINT PRIMARY KEY AUTO_INCREMENT, OrdersID BIGINT,
						FOREIGN KEY (OrdersID) REFERENCES orders(OrdersID));
                        
                        
                        
INSERT INTO customer (FirstName, LastName, PhoneNumber) VALUES ('Nobert', 'Mika', 4165551111);
INSERT INTO customer (FirstName, LastName, PhoneNumber) VALUES ('Russell', 'Foubert', 5195552222);
INSERT INTO customer (FirstName, LastName, PhoneNumber) VALUES ('Sean', 'Clarke', 5195553333);
SELECT * FROM customer;

INSERT INTO branch (BranchName) VALUES ('Sports World');
INSERT INTO branch (BranchName) VALUES ('Cambridge Mall');
INSERT INTO branch (BranchName) VALUES ('St.jacobs');
SELECT * FROM branch;

INSERT INTO product (ProductName, Price, InventoryQuantity) VALUES ('Disco Queen Wallpaper (roll)', 12.95, 56);
INSERT INTO product (ProductName, Price, InventoryQuantity) VALUES ('Countryside Wallpaper (roll)', 11.95, 24);
INSERT INTO product (ProductName, Price, InventoryQuantity) VALUES ('Victorian Lace Wallpaper (roll)', 14.95, 44);
INSERT INTO product (ProductName, Price, InventoryQuantity) VALUES ('Drywall Tape (roll)', 3.95, 120);
INSERT INTO product (ProductName, Price, InventoryQuantity) VALUES ('Drywall Tape (pkg 10)', 36.95, 30);
INSERT INTO product (ProductName, Price, InventoryQuantity) VALUES ('Drywall Repair Compound (tube)', 6.95, 90);
SELECT * FROM product;