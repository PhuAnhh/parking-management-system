CREATE TABLE customer_groups(
	id int identity(1, 1) primary key,
	code nvarchar(255) not null,
	name nvarchar(255) not null,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
)

CREATE TABLE customers(
	id int identity(1, 1) primary key,
	code nvarchar(255) not null,
	name nvarchar(255) not null,
	phone nvarchar(20),
	address nvarchar(255),
	customer_group_id int,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (customer_group_id) references customer_groups(id),
)
 
CREATE TABLE card_groups(
	id int identity(1, 1) primary key,
	code nvarchar(255) not null,
	name nvarchar(255) not null,
	type nvarchar(255) check (type in ('monthly', 'daily')),
	status bit not null default 1, 
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
)

CREATE TABLE cards(
	id int identity(1, 1) primary key,
	code nvarchar(255) not null,
	name nvarchar(255) not null,
	card_group_id int not null, 
	customer_id int not null,
	note nvarchar(255),
	status nvarchar(255) not null check(status in('in_use', 'unused', 'locked')),
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (card_group_id) references card_groups(id),
	foreign key (customer_id) references customers(id)
)


CREATE TABLE entry_logs(
	id int identity(1, 1) primary key,
	plate_number nvarchar(50),
	card_id int not null,
	customer_id int,
	creator nvarchar(255),
	vehicle_type nvarchar(50) not null check (vehicle_type in ('car', 'motorbike', 'bicycle')),
	entry_time datetime not null default getdate(),
	entry_lane nvarchar(255) not null,
	image_url nvarchar(255) not null,
	note nvarchar(255),
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (card_id) references cards(id),
	foreign key (customer_id) references customers(id)
)

CREATE TABLE exit_logs(
	id int identity(1, 1) primary key, 
	plate_number_entry nvarchar(50) not null,
	plate_number_exit nvarchar(50) not null,
	card_id int not null,
	customer_id int,
	entry_time datetime not null,
	exit_time datetime default getdate(),	
	creator nvarchar(255),
	entry_id int not null,
	vehicle_type nvarchar(50) not null check (vehicle_type in ('car', 'motorbike', 'bicycle')),
	exit_lane nvarchar(255) not null,
	image_url nvarchar(200) not null,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (entry_id) references entry_logs(id),
	foreign key (card_id) references cards(id),
	foreign key (customer_id) references customers(id)
)