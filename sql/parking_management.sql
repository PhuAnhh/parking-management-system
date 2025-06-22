CREATE TABLE gates(
	id int identity(1, 1) primary key, 
	name nvarchar(255) not null,
	code nvarchar(255) not null, 
	status bit not null default 1,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null
)

CREATE TABLE computers(
	id int identity(1, 1) primary key,
	name nvarchar(255) not null,
	ip_address nvarchar(255) not null,
	gate_id int not null,	
	status bit not null default 1,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (gate_id) references gates(id)
)

CREATE TABLE cameras(
	id int identity(1, 1) primary key,
	name nvarchar(255) not null,
	code nvarchar(255) not null,
	ip_address nvarchar(255) not null,
	resolution nvarchar(50) null,
	type nvarchar(225) null,
	username nvarchar(255) null,
	password nvarchar(255) null,
	computer_id int not null,
	status bit not null default 1,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (computer_id) references computers(id)
)

CREATE TABLE control_units(
	id int identity(1, 1) primary key,
	name nvarchar(255) not null,
	code nvarchar(255) not null,
	username nvarchar(255) not null,
	password nvarchar(255) not null,
	comport nvarchar(255) null, 
	baudrate int null,
	type nvarchar(255) null,
	connection_protocol nvarchar(255) not null,
	computer_id int not null,
	status bit not null default 1,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (computer_id) references computers(id)
)

CREATE TABLE lanes(
	id int identity(1, 1) primary key,
	name nvarchar(255) not null,
	code nvarchar(255) not null,
	type nvarchar(255) not null,
	reverse_lane int null,
	computer_id int not null,
	auto_open_barrier nvarchar(255) not null,
	loop bit not null,
	display_led bit not null,
	status bit not null default 1,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (computer_id) references computers(id)
)

CREATE TABLE leds(
	id int identity(1, 1) primary key, 
	name nvarchar(255) not null,
	code nvarchar(255) not null,
	computer_id int not null,
	comport nvarchar(255), 
	baudrate int not null,
	type nvarchar(255) not null,
	status bit not null default 1,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (computer_id) references computers(id)
)

CREATE TABLE lane_cameras(
	id int identity(1, 1) not null primary key,
    lane_id int not null,
    camera_id int not null,
    purpose nvarchar(255) not null,
	display_position int not null,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
   	foreign key (lane_id) references lanes(id),
    foreign key (camera_id) references cameras(id)
)

CREATE TABLE lane_control_units(
	id int identity(1, 1) not null primary key,
	lane_id int not null,
	control_unit_id int not null,
	reader nvarchar(255) null,
	input nvarchar(255) null,
	barrier nvarchar(255) null,
	alarm nvarchar(255) null,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
    foreign key (lane_id) references lanes(id),
    foreign key (control_unit_id) references control_units(id)
)

CREATE TABLE customer_groups(
	id int identity(1, 1) primary key,
	code nvarchar(255) not null,
	name nvarchar(255) not null,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null
)

CREATE TABLE customers(
	id int identity(1, 1) primary key,
	code nvarchar(255) not null,
	name nvarchar(255) not null,
	phone nvarchar(255),
	address nvarchar(255),
	customer_group_id int,
	deleted bit not null default 0,	
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (customer_group_id) references customer_groups(id)
)

CREATE TABLE card_groups(
	id int identity(1, 1) primary key,
	code nvarchar(255) not null,
	name nvarchar(255) not null,
	type nvarchar(255) not null,
	vehicle_type nvarchar(255) not null,
	free_minutes INT NULL,
	first_block_minutes INT NULL,
	first_block_price DECIMAL(18,2) NULL,
	next_block_minutes INT NULL,
	next_block_price DECIMAL(18,2) NULL,
	status bit not null default 1, 
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null
)

CREATE TABLE card_group_lanes(
	id int identity(1, 1) primary key,
	card_group_id int not null,
	lane_id int not null,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (card_group_id) references card_groups(id),
	foreign key (lane_id) references lanes(id)	
)

CREATE TABLE cards(
	id int identity(1, 1) primary key,
	code nvarchar(255) not null,
	name nvarchar(255) not null,
	card_group_id int not null, 
	customer_id int null,
	note nvarchar(255),
	start_date datetime null,
    end_date datetime null,
	auto_renew_days int null,
	status nvarchar(255) not null,
	deleted bit not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (card_group_id) references card_groups(id),
	foreign key (customer_id) references customers(id)
)

CREATE TABLE entry_logs(
	id int identity(1, 1) primary key,
	plate_number nvarchar(255),
	card_id int not null,
	card_group_id int not null,
	lane_id int not null,
	customer_id int,
	entry_time datetime not null default getdate(),
	image_url nvarchar(255) null,
	note nvarchar(255) null,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (card_id) references cards(id),
	foreign key (card_group_id) references card_groups(id),
	foreign key (customer_id) references customers(id),
	foreign key (lane_id) references lanes(id)
)

CREATE TABLE exit_logs (
    id int identity(1,1) primary key,
    entry_log_id int not null unique, 
    exit_plate_number nvarchar(255) null, 
    exit_lane_id int not null, 
    exit_time datetime not null, 
    total_duration bigint not null,
    total_price decimal(18,2) not null, 
    note nvarchar(255) null, 
    image_url nvarchar(255) null, 
    created_at datetime default getdate() not null, 
    foreign key (entry_log_id) references entry_logs(id),
    foreign key (exit_lane_id) references lanes(id) 
)

CREATE TABLE warning_events (
    id int identity(1, 1) primary key,           
    plate_number nvarchar(50) null,             
    lane_id int not null,                              
    warning_type nvarchar(255) null,         
    note nvarchar(255) null,                          
    created_at datetime default getdate() not null, 
    foreign key (lane_id) references lanes(id)   
)

CREATE TABLE revenue_reports(
	id int identity(1, 1) primary key,
	card_group_id int not null, 
	exit_count int not null default 0,
	revenue decimal(18, 2) not null default 0.00,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (card_group_id) references card_groups(id)
)

CREATE TABLE roles(
	id int identity(1, 1) primary key,
	name nvarchar(255) not null,
	description nvarchar(255) null,
	deleted bit  not null default 0,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null
)

CREATE TABLE users(
	 id int identity(1, 1) primary key,
	 username nvarchar(255) not null unique,
	 password nvarchar(255) not null,
	 name nvarchar(255) null,
	 role_id int not null,
	 status bit not null default 1,
	 deleted bit not null default 0,
	 created_at datetime default getdate() not null,
	 updated_at datetime default getdate() not null,
	 foreign key (role_id) references roles(id)
)

CREATE TABLE permissions(
	id int identity(1, 1) primary key,
	name nvarchar(255) not null,
	endpoint nvarchar(255) not null,
	method nvarchar(10) not null,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null
)

CREATE TABLE role_permissions(
	id int identity(1, 1) primary key,
	role_id int not null,
	permission_id int not null,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (role_id) references roles(id),
	foreign key (permission_id) references permissions(id)
)