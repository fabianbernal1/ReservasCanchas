GO
CREATE PROCEDURE sp_GuardarReserva
(
    @ReservaId     INT,
    @UsuarioId     INT,
    @CanchaId      INT,
    @FechaReserva  DATETIME2,
    @HoraInicio    TIME,
    @HoraFin       TIME,
    @EstadoId      INT
)
AS
BEGIN
    SET NOCOUNT ON;

    --------------------------------------------------------------
    -- 1. VALIDAR QUE NO HAYA RESERVAS SOLAPADAS
    --------------------------------------------------------------
    IF EXISTS (
        SELECT 1 
        FROM Reservas
        WHERE 
            CanchaId = @CanchaId
            AND CAST(FechaReserva AS DATE) = CAST(@FechaReserva AS DATE)
            
            -- Validación de cruce de horarios
            AND (
                HoraInicio < @HoraFin AND HoraFin > @HoraInicio
            )
            
            -- Evitar comparar consigo mismo al editar
            AND (@ReservaId = 0 OR ReservaId <> @ReservaId)
    )
    BEGIN
        RAISERROR('Existe otra reserva solapada para esa cancha en esa fecha y horario.', 16, 1);
        RETURN;
    END

    --------------------------------------------------------------
    -- 2. INSERTAR NUEVA RESERVA
    --------------------------------------------------------------
    IF @ReservaId = 0
    BEGIN
        INSERT INTO Reservas (
            UsuarioId, CanchaId, FechaReserva,
            HoraInicio, HoraFin, EstadoId
        )
        VALUES (
            @UsuarioId, @CanchaId, @FechaReserva,
            @HoraInicio, @HoraFin, @EstadoId
        );

        SELECT SCOPE_IDENTITY() AS NuevaReservaId; -- Devuelve Id
        RETURN;
    END

    --------------------------------------------------------------
    -- 3. ACTUALIZAR RESERVA EXISTENTE
    --------------------------------------------------------------
    UPDATE Reservas
    SET 
        UsuarioId = @UsuarioId,
        CanchaId = @CanchaId,
        FechaReserva = @FechaReserva,
        HoraInicio = @HoraInicio,
        HoraFin = @HoraFin,
        EstadoId = @EstadoId
    WHERE ReservaId = @ReservaId;

END
GO
