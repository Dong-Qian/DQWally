-- Ownner:       Dong Qian (6573448)
-- Script:       RD assignment 04
-- Due Date:     Dec 3, 2016
-- Instructor:   Russell Foubert
-- Description:  Start Data Base for RevengeOfWally assignemnt


CREATE DATABASE DQWally;

USE DQWally;
grant all privileges on dqwally.* to 'super'@'%' identified by 'Conestoga1';

#Create the tables
#customer table
CREATE TABLE customer (CustomerID BIGINT PRIMARY KEY AUTO_INCREMENT, FirstName VARCHAR(45),
					   LastName VARCHAR(45), PhoneNumber VARCHAR(45));

# branch table
CREATE TABLE branch (BranchID BIGINT PRIMARY KEY AUTO_INCREMENT, BranchName VARCHAR(45));

# product table
CREATE TABLE product (ProductID BIGINT PRIMARY KEY AUTO_INCREMENT, ProductName VARCHAR(45), 
					  UnitPrice DECIMAL(10,2), InventoryQuantity BIGINT);

# orders table
CREATE TABLE orders (OrdersID BIGINT PRIMARY KEY AUTO_INCREMENT, OrderDate DATETIME, 
					 CustomerID BIGINT, BranchID BIGINT, OrderStatus VARCHAR(10),
                     FOREIGN KEY (CustomerID) REFERENCES customer(CustomerID),
					 FOREIGN KEY (BranchID) REFERENCES branch(BranchID));

# orderline table
CREATE TABLE orderline (OrdersID BIGINT, ProductID BIGINT, Quantity BIGINT,
						FOREIGN KEY (OrdersID) REFERENCES orders(OrdersID),
						FOREIGN KEY (ProductID) REFERENCES product(ProductID));
                        
                        
# insert 3 customer to customer table
INSERT INTO customer (FirstName, LastName, PhoneNumber) VALUES ('Nobert', 'Mika', '416-555-1111'),
															   ('Russell', 'Foubert', '519-555-2222'),
															   ('Sean', 'Clarke', '519-555-3333');


# insert 3 branch into branch table
INSERT INTO branch (BranchName) VALUES ('Sports World'),
									   ('Cambridge Mall'),
									   ('St.jacobs');
                                       

# insert 6 product into product table
INSERT INTO product (ProductName, UnitPrice, InventoryQuantity) VALUES ('Disco Queen Wallpaper (roll)', 12.95, 56),
																   ('Countryside Wallpaper (roll)', 11.95, 24),
																   ('Victorian Lace Wallpaper (roll)', 14.95, 44),
																   ('Drywall Tape (roll)', 3.95, 120),
																   ('Drywall Tape (pkg 10)', 36.95, 30),
																   ('Drywall Repair Compound (tube)', 6.95, 90);


# insert 3 orders to orders table
INSERT INTO orders(OrderDate, CustomerID, BranchID, OrderStatus) VALUES ('2016-09-20', 3, 1, 'PAID'),
																		('2016-10-06', 2, 2, 'PEND'),
                                                                        ('2016-11-02', 1, 3, 'PAID');
                                                                        
# insert orderline for sean's order
INSERT INTO orderline (OrdersID, ProductID, Quantity) VALUES (1, 3, 4),
															 (1, 6, 1),
															 (1, 4, 2 );
                                                             
# change InventoryQuantity for Sean's order
UPDATE product SET InventoryQuantity = InventoryQuantity - 4 WHERE ProductID = 3;
UPDATE product SET InventoryQuantity = InventoryQuantity - 1 WHERE ProductID = 6;
UPDATE product SET InventoryQuantity = InventoryQuantity - 2 WHERE ProductID = 4;


# insert orderline for Russell's order										
INSERT INTO orderline (OrdersID, ProductID, Quantity) VALUES (2, 2, 10);

# insert orderline for Norbert's order
INSERT INTO orderline (OrdersID, ProductID, Quantity) VALUES (3, 1, 12),
															 (3, 4, 3);

# change InventoryQuantity for Norbert's order 
UPDATE product SET InventoryQuantity = InventoryQuantity - 12 WHERE ProductID = 1;
UPDATE product SET InventoryQuantity = InventoryQuantity - 3 WHERE ProductID = 4;


# change the order status for Russel's pend order
UPDATE orders SET OrderStatus = 'CNCL' WHERE OrdersID = 2;
# zero out the orderline
UPDATE orderline SET Quantity = 0 WHERE OrdersID = 2 AND ProductID = 2 AND Quantity = 10;


															
# change the order status fro Norber's order
UPDATE orders SET OrderStatus = 'RFND' WHERE OrdersID = 3;
# adjust the inventory
UPDATE product SET InventoryQuantity = InventoryQuantity + 12 WHERE ProductID = 1;
UPDATE product SET InventoryQuantity = InventoryQuantity + 3 WHERE ProductID = 4;
# zero out the orderline
UPDATE orderline SET Quantity = 0 WHERE OrdersID = 3 AND ProductID = 1 AND  Quantity = 12;
UPDATE orderline SET Quantity = 0 WHERE OrdersID = 3 AND ProductID = 4 AND  Quantity = 3;
												