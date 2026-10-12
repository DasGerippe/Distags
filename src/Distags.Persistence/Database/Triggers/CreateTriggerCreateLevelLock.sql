create or replace function create_level_lock() returns trigger as $$
begin
    insert into level_locks (level_id) values (new.id);
    return new;
end;
$$ language plpgsql;

drop trigger if exists create_level_lock on levels;

create trigger create_level_lock
after insert on levels
for each row
execute function create_level_lock();
