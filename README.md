# Buggy Bookings API

## DI och DRY

Projektet tillämpar Dependency Injection genom att BookingsController får IBookingService injicerat via konstruktorn, vilket möjliggör löst kopplade beroenden.
BookingService är i sin tur beroende av IBookingRepository som hanterar datalagringen. Båda interfaces är registrerade med AddScoped i Program.cs.

DRY-principen är tillämpad genom att all överlappslogik är samlad i en enda metod – HasOverlap() i BookingService.
Controllern använder enbart Create() och låter HasOverlap() i BookingService avgöra om en bokning kan accepteras eller ej. Detta eliminerar duplicerad logik i enlighet med DRY. 

---
## 1. Bug #1 – dubbelbokning

### Uppgift:
> Tidsöverlapp accepteras ibland. Hitta roten och åtgärda så att testet `CreateBooking_RejectsOverlap2` passerar.

### Lösning
Testet `CreateBooking_RejectsOverlap2()` används för att kontrollera att bokningar som **slutar exakt när nästa börjar** (t.ex. slutar 12:00, nästa börjar 12:00) **inte räknas som ett tidsöverlapp**, de ska alltså vara tillåtna.

I metoden `HasOverlap()` fanns följande logik:
```
return _repo.GetAll().Any(b => b.RoomId == roomId && !(b.To <= from || b.From > to));
```
Villkoret är för strikt eftersom det inte tillåter att en bokning får börja precis när en annan slutar. Det ändrades därför till:
```
return _repo.GetAll().Any(b => b.RoomId == roomId && !(b.To <= from || b.From >= to));
```
## 2. Bug #2 – DI‑kringgås

### Uppgift:
>   `BookingController` instansierar `BookingService` direkt. Flytta till DI‑containern så att beroenden injiceras.

### Lösning
BookingService skapades tidigare direkt i kontrollern:
```
private readonly BookingService _service = new BookingService();
```
Koden ändrades så att den istället tar emot BookingService via en konstruktor:
```
private readonly BookingService _service;

public BookingsController(BookingService service)
{
    _service = service;
}
```
Därefter registrerades både BookingService och BookingRepository i DI-containern i Program.cs.

## 3. Ny funktion – avboka

### Uppgift:
>  Implementera `DELETE /api/bookings/{id}` med 204 No Content på lyckad avbokning och 404 om id saknas.

### Lösning
En ny delete-metod Cancel(int id) har lagts till i controllern. Saknas bokningen returneras 404 Not Found. Finns den, avbokas den och returnerar 204 No Content:
```
   [HttpDelete("{id:int}")]
   public IActionResult Cancel(int id)
   {
       var existing = _service.GetById(id);

       if (existing == null)
       {
           return NotFound();
       }

       _service.Cancel(id);
       return NoContent();
   }
```

## 4. Refaktorera

### Uppgift: 
Inför `IBookingService` och `IBookingRepository`. 
All överlappslogik ska finnas på **ett** ställe.  

### Lösning
- Både BookingService och BookingRepository implementerar nu sina respektive interfaces, och controllern använder IBookingService.

- Create-metoden i controllern ändrades från detta:
```
[HttpPost]
public IActionResult Create([FromBody] Booking booking)
{
    if (_service.HasOverlap(booking.RoomId, booking.From, booking.To))
    {
        return Conflict("Booking overlaps an existing reservation.");
    }

    var created = _service.Create(booking);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```
till detta:
```
[HttpPost]
public IActionResult Create([FromBody] Booking booking)
{
    try
    {
        var created = _service.Create(booking);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(ex.Message);
    }
}
```
Nu frågar controllern inte om det finns en överlapp, den försöker bara skapa bokningen. Om BookingService upptäcker ett överlapp, kastar den ett fel.
Resultatet blir att all överlappslogik finns på ett enda ställe (i service-lagret). 

