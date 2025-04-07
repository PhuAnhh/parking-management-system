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
	id int identity(1, 1) primary key,
    lane_id int not null,
    camera_id int not null,
    purpose nvarchar(50) not null check (purpose in ('Motorbike Plate', 'Overview', 'Car Plate')),
	display_position int not null check (display_position in ('0', '1', '2', '3')),
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
    foreign key (lane_id) references lanes(id),
    foreign key (camera_id) references cameras(id),
)

CREATE TABLE lane_control_units(
	id int identity(1, 1) primary key,
	lane_id int not null,
	control_unit_id int not null,
	reader int not null check (reader in ('0', '1', '2', '3', '4', '5')),
	input char(1) not null check (input in ('0', '1', '2', '3', '4', '5')),
	barrier char(1) not null check (barrier in ('0', '1', '2', '3', '4', '5')),
	alarm char(1) not null check (alarm in ('0', '1', '2', '3', '4', '5')),
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
    foreign key (lane_id) references lanes(id),
    foreign key (control_unit_id) references control_units(id)
)
