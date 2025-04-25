## 🩺 Funcionamiento de la Simulación y Estrategias de Atención - QuickCareSim

Este documento describe cómo funciona la simulación en QuickCareSim y los tipos de estrategias de atención disponibles. Se detallan las diferencias entre ejecuciones secuenciales y paralelas, así como el propósito y lógica de cada estrategia.

## ⚙️ Funcionamiento General de la Simulación

Una simulación médica en QuickCareSim consiste en la atención de un conjunto de pacientes por uno o varios doctores, de manera secuencial o paralela. El sistema mide métricas de rendimiento como:

Tiempo real de ejecución

Pacientes atendidos

Tiempo promedio por paciente

Speedup y eficiencia (en modo paralelo) 


-----------------------------------------------------------------------------------------------------------


## 🔄 Flujo de Ejecución

Se generan pacientes aleatoriamente con distintos niveles de urgencia.

Se asignan doctores disponibles.

Se selecciona una estrategia de atención.

Cada doctor atiende pacientes de la cola según la estrategia seleccionada.

Se registran logs de atención y se generan métricas.


-----------------------------------------------------------------------------------------------------------


## 🧵 Tipos de Ejecución

▶️ Simulación Secuencial

Un solo doctor (Procesador) atiende a todos los pacientes.

No hay paralelismo.

Útil para comparar contra ejecuciones paralelas (calcular speedup).

🚀 Simulación Paralela

Múltiples doctores (Procesador) trabajan en paralelo.

Cada uno procesa pacientes de la cola compartida.

Maximiza uso de CPU con Parallel.ForEachAsync y la ejecucion de multiples hilos.


-----------------------------------------------------------------------------------------------------------


## 🧠 Estrategias de Atención Disponibles

Las estrategias controlan el orden en que los pacientes son seleccionados desde la cola compartida.

-----------------------------------------------------------------------------------------------------------

## 1. RoundRobinStrategy

Lógica: Atiende a los pacientes en el orden en que llegan (FIFO).

Ventaja: Justa y sencilla.

Uso: Ideal para cargas homogéneas.


-----------------------------------------------------------------------------------------------------------

## 2. PriorityStrategy

Lógica: Atiende primero a los pacientes con mayor urgencia.

Ventaja: Mejora el tiempo de atención a casos críticos.

Uso: Útil en entornos con alta variabilidad en urgencias.


-----------------------------------------------------------------------------------------------------------

## 3. EmergencyTypeStrategy

Lógica: Ordena y prioriza a pacientes según tipo de emergencia (urgencia numérica).

Ventaja: Optimiza la atención con base en gravedad médica.

Uso: Similar a PriorityStrategy pero con enfoque en orden clínico más estricto.

Cada estrategia implementa la interfaz IAttentionStrategyService y se inyecta dinámicamente mediante el AttentionStrategyFactory según los parámetros de simulación.

📈 Registro y Métricas

Durante la ejecución se generan:

AttentionLog: Información de cada atención (doctor, paciente, inicio/fin).

PerformanceMetric: Número de pacientes por doctor y tiempo promedio.

UrgencyWaitMetric: Promedio de espera agrupado por nivel de urgencia.

Estas métricas son usadas luego en informes, visualizaciones y comparación entre estrategias.

-----------------------------------------------------------------------------------------------------------

##  🧪 Comparación de Resultados

La simulación guarda resultados históricos que permiten comparar:

Eficiencia de estrategias

Beneficio del paralelismo (speedup y eficiencia)

Tiempos de espera por urgencias.

Esto permite tomar decisiones informadas sobre qué estrategia utilizar bajo distintos escenarios.

-----------------------------------------------------------------------------------------------------------
