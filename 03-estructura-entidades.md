## 🧾 Entidades

Este documento describe las entidades del dominio utilizadas en el sistema, así como una breve explicación de sus atributos y propósito.


-----------------------------------------------------------------------------------------------------------

## 🧩 1. Entidad: SimulationRun

Representa una ejecución completa de una simulación, registrando parámetros, resultados y métricas.

🔍 Propiedades:

Id: Identificador único de la simulación.

RunAt: Fecha y hora en que se ejecutó la simulación.

StrategyUsed: Estrategia de atención aplicada.

TotalDoctors, TotalPatients: Número de doctores y pacientes usados.

ExecutionTimeSeconds: Tiempo de ejecución en segundos (lógico).

RealExecutionTimeSeconds: Tiempo real medido por el sistema.

TotalPatientsAttended: Pacientes atendidos efectivamente.

ProcessorsUsed: Núcleos paralelos utilizados.

Speedup, Efficiency: Métricas de rendimiento para evaluar paralelismo.

🔗 Relaciones: Uno a muchos con PerformanceMetric, UrgencyWaitMetric, AttentionLog.


-----------------------------------------------------------------------------------------------------------

## 🧑‍⚕️ 2. Entidad: Doctor

Representa a un profesional médico disponible para atender pacientes durante una simulación.

🔍 Propiedades:

UserId: Identificador del doctor.

Status: Estado del doctor (AVAILABLE, BUSY).

🔗 Relaciones: Atendió múltiples pacientes y se asocian métricas de desempeño.


-----------------------------------------------------------------------------------------------------------

## 🧑‍🦽 3. Entidad: Patient

Paciente generado con urgencia específica, que será atendido durante la simulación.

🔍 Propiedades:

UserId: Identificador del paciente.

Urgency: Nivel de urgencia médica.

ArrivalTime: Hora de llegada a la sala.

AttendedTime: Hora de inicio de la atención médica.

Status: Estado actual (esperando, en atención, atendido).

🔗 Relaciones: Cada paciente puede estar relacionado con un único doctor.


-----------------------------------------------------------------------------------------------------------

## 📝 4. Entidad: AttentionLog

Historial detallado de cada atención médica realizada, con tiempos e identificadores de los involucrados.

🔍 Propiedades:

Id: Identificador del log.

PatientId, DoctorId: Identificadores del paciente y doctor.

StartTime, EndTime: Inicio y fin de la atención.

SimulationRunId: Simulación a la que pertenece.

StrategyUsed: Estrategia usada en esa atención.

🔗 Relaciones: Asociado a una única simulación.


-----------------------------------------------------------------------------------------------------------

## 📊 5. Entidad: PerformanceMetric

Mide el rendimiento individual de cada doctor en una simulación.

🔍 Propiedades:

Id: Identificador de la métrica.

DoctorId: Doctor asociado.

SimulationRunId: Simulación correspondiente.

PatientsAttended: Número de pacientes atendidos.

AverageAttentionTimeSeconds: Tiempo promedio por atención.


-----------------------------------------------------------------------------------------------------------

## ⏱ 6. Entidad: UrgencyWaitMetric

Resume el tiempo de espera promedio de pacientes agrupados por nivel de urgencia.

🔍 Propiedades:

Id: Identificador de la métrica.

UrgencyLevel: Nivel de urgencia.

AverageWaitSeconds: Promedio de espera en segundos.

TotalPatients: Pacientes que componen esta métrica.

SimulationRunId: Simulación asociada.


-----------------------------------------------------------------------------------------------------------
