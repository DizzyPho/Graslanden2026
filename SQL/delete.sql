truncate table measurement;
delete from species;
delete from inventoried_plot;
delete from grass_plot;
delete from management_type;
delete from inventory;

insert into management_type (type) values ('Intensief'), ('Extensief'), ('Netheidsboord'), ('Schapenweide')