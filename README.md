# Buggy Bookings API

- [Bug #1 – Dubbelbokning](#bug-1--dubbelbokning)

---

## Bug #1 – dubbelbokning

### Uppgift:
> Tidsöverlapp accepteras ibland. Hitta roten och åtgärda så att testet `CreateBooking_RejectsOverlap2` passerar.

### Lösning:
Testet `CreateBooking_RejectsOverlap2()` används för att kontrollera att bokningar som **slutar exakt när nästa börjar** (t.ex. slutar 12:00, nästa börjar 12:00) **inte räknas som ett tidsöverlapp**, de ska alltså vara tillåtna.

I metoden `HasOverlap()` fanns följande logik:
```
return _repo.GetAll().Any(b => b.RoomId == roomId && !(b.To <= from || b.From > to));
```
Villkoret är för strikt eftersom det inte tillåter att en bokning får börja precis när en annan slutar. Det ändrades därför till:
```
return _repo.GetAll().Any(b => b.RoomId == roomId && !(b.To <= from || b.From >= to));
```
