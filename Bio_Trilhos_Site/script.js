// ============================================================================
// BIO TRILHOS - Dashboard Frontend
// Integração com API REST do backend
// ============================================================================

// Configuração da API
const API_BASE_URL = "http://localhost:5000/api";
const ESTUFA_ID = 1; // ID da estufa padrão

// Elementos DOM
const navItems = document.querySelectorAll(".nav-item");
const sections = document.querySelectorAll(".page-section");
const pageTitle = document.getElementById("page-title");
const titles = {dashboard:"Dashboard", sensores:"Sensores", irrigacao:"Irrigação", clima:"Clima", historico:"Histórico"};

// Variáveis globais
let autoMode = true;
let lastUpdateTime = new Date();

// ============================================================================
// NAVEGAÇÃO
// ============================================================================

navItems.forEach(item => {
  item.addEventListener("click", () => {
    const target = item.dataset.section;
    navItems.forEach(n => n.classList.remove("active"));
    item.classList.add("active");
    sections.forEach(s => s.classList.toggle("active-section", s.id === target));
    pageTitle.textContent = titles[target];
    
    // Carregar dados específicos da seção
    if (target === "dashboard") loadDashboard();
    else if (target === "sensores") loadSensores();
    else if (target === "clima") loadClima();
  });
});

// ============================================================================
// FUNÇÕES AUXILIARES
// ============================================================================

function toast(message) {
  const t = document.getElementById("toast");
  t.textContent = message;
  t.classList.add("show");
  setTimeout(() => t.classList.remove("show"), 2200);
}

function formatarData(dataISO) {
  const data = new Date(dataISO);
  const agora = new Date();
  const diff = Math.floor((agora - data) / 1000);
  
  if (diff < 60) return "agora";
  if (diff < 3600) return `${Math.floor(diff / 60)}m atrás`;
  if (diff < 86400) return `${Math.floor(diff / 3600)}h atrás`;
  
  return data.toLocaleDateString("pt-BR");
}

async function fetchAPI(endpoint, options = {}) {
  try {
    const url = `${API_BASE_URL}${endpoint}`;
    const response = await fetch(url, {
      method: options.method || "GET",
      headers: {
        "Content-Type": "application/json",
        ...options.headers
      },
      body: options.body ? JSON.stringify(options.body) : undefined
    });
    
    if (!response.ok) {
      throw new Error(`Erro ${response.status}: ${response.statusText}`);
    }
    
    return await response.json();
  } catch (error) {
    console.error(`Erro ao chamar API ${endpoint}:`, error);
    toast(`Erro: ${error.message}`);
    return null;
  }
}

// ============================================================================
// DASHBOARD
// ============================================================================

async function loadDashboard() {
  try {
    const dados = await fetchAPI(`/dashboard/${ESTUFA_ID}`);
    
    if (!dados) {
      toast("Erro ao carregar dados do dashboard");
      return;
    }
    
    // Atualizar temperatura
    document.getElementById("temperature").textContent = dados.temperaturaAtual.toFixed(1) + "°C";
    document.getElementById("tempBar").style.width = Math.min(100, (dados.temperaturaAtual / 40) * 100) + "%";
    
    // Atualizar umidade do ar
    document.getElementById("airHumidity").textContent = Math.round(dados.umidadeArAtual) + "%";
    document.getElementById("airBar").style.width = Math.min(100, dados.umidadeArAtual) + "%";
    
    // Atualizar umidade do solo
    document.getElementById("soilHumidity").textContent = Math.round(dados.umidadeSoloAtual) + "%";
    document.getElementById("soilBar").style.width = Math.min(100, dados.umidadeSoloAtual) + "%";
    document.getElementById("sensorSoil").textContent = Math.round(dados.umidadeSoloAtual) + "%";
    
    // Atualizar nível do reservatório
    document.getElementById("waterLevel").textContent = Math.round(dados.reservatorioPercentual) + "%";
    document.getElementById("waterBar").style.width = Math.min(100, dados.reservatorioPercentual) + "%";
    document.getElementById("tankFill").style.height = Math.min(100, dados.reservatorioPercentual) + "%";
    document.getElementById("tankText").textContent = Math.round(dados.reservatorioPercentual) + "%";
    document.getElementById("availableLiters").textContent = Math.round(dados.reservatorioNivel) + " L";
    
    // Atualizar irrigação
    document.getElementById("irrigationStatus").textContent = dados.irrigacaoAtiva 
      ? "Irrigação em progresso" 
      : "Irrigação em espera";
    
    if (dados.ultimaIrrigacao) {
      const dataIrr = new Date(dados.ultimaIrrigacao);
      const horas = String(dataIrr.getHours()).padStart(2, '0');
      const minutos = String(dataIrr.getMinutes()).padStart(2, '0');
      document.getElementById("lastIrrigation").textContent = `Hoje, ${horas}:${minutos}`;
    }
    
    // Atualizar meteorologia
    document.getElementById("weatherTemp").textContent = Math.round(dados.temperaturaExterna) + "°C";
    document.getElementById("rainChance").textContent = (dados.probabilidadeChuva || 0).toFixed(0) + "%";
    
    // Atualizar hora da última atualização
    document.getElementById("lastUpdate").textContent = formatarData(dados.ultimaAtualizacao);
    lastUpdateTime = new Date(dados.ultimaAtualizacao);
    
  } catch (error) {
    console.error("Erro ao carregar dashboard:", error);
  }
}

function makeChart() {
  const chart = document.getElementById("chart");
  const values = [51, 55, 58, 61, 60, 64, 68, 65, 62, 64, 66, 64];
  chart.innerHTML = values.map(v => `<span style="height:${v * 1.25}px" title="${v}%"></span>`).join("");
}

// ============================================================================
// SENSORES
// ============================================================================

async function loadSensores() {
  try {
    const sensores = await fetchAPI(`/sensores?estufaId=${ESTUFA_ID}`);
    
    if (!sensores || sensores.length === 0) {
      toast("Nenhum sensor encontrado");
      return;
    }
    
    // Atualizar lista de sensores com dados reais
    const tipoSensorMap = {0: "🌡️", 1: "💨", 2: "🌱", 3: "💡", 4: "🌧️", 5: "💧"};
    const tipoNomeMap = {
      0: "Temperatura",
      1: "Umidade do Ar",
      2: "Umidade do Solo",
      3: "Luminosidade",
      4: "Sensor de Chuva",
      5: "Nível de Água"
    };
    
    const sensorList = document.querySelector(".sensor-list");
    sensorList.innerHTML = sensores.map(sensor => {
      const emoji = tipoSensorMap[sensor.tipoSensor] || "⚙️";
      const statusClass = sensor.ativo ? "good" : "warning";
      const statusText = sensor.ativo ? "Online" : "Offline";
      
      return `
        <div class="sensor-row">
          <span class="sensor-circle">${emoji}</span>
          <div>
            <strong>${sensor.nome}</strong>
            <small>${tipoNomeMap[sensor.tipoSensor]}</small>
          </div>
          <b>--</b>
          <span class="status ${statusClass}">${statusText}</span>
        </div>
      `;
    }).join("");
    
  } catch (error) {
    console.error("Erro ao carregar sensores:", error);
  }
}

// ============================================================================
// CLIMA
// ============================================================================

async function loadClima() {
  try {
    const clima = await fetchAPI(`/meteorologia/atual/${ESTUFA_ID}`);
    
    if (!clima) {
      toast("Erro ao carregar dados meteorológicos");
      return;
    }
    
    // Determinar emoji da condição climática
    const climaEmoji = clima.estaChovendo ? "🌧️" : 
                       clima.condicaoClimatica.includes("Chuva") ? "🌦️" :
                       clima.condicaoClimatica.includes("Nublado") ? "☁️" :
                       clima.condicaoClimatica.includes("limpo") ? "☀️" : "🌤️";
    
    // Card principal
    document.querySelector(".weather-big").innerHTML = 
      `${climaEmoji} <strong>${Math.round(clima.temperaturaExterna)}°C</strong>`;
    
    // Previsão
    const rainPercentual = clima.probabilidadeChuva || 0;
    document.querySelector(".rain-number").innerHTML = 
      `${rainPercentual.toFixed(0)}%<small>chance de chuva</small>`;
    
  } catch (error) {
    console.error("Erro ao carregar clima:", error);
  }
}

// ============================================================================
// IRRIGAÇÃO
// ============================================================================

document.getElementById("autoToggle").addEventListener("change", async (e) => {
  autoMode = e.target.checked;
  document.getElementById("irrigationStatus").textContent = autoMode 
    ? "Irrigação em espera" 
    : "Automação desativada";
  toast(autoMode ? "Modo automático ativado." : "Modo automático desativado.");
});

document.getElementById("manualIrrigation").addEventListener("click", async () => {
  try {
    const resultado = await fetchAPI("/irrigacao/iniciar", {
      method: "POST",
      body: {
        estufaId: ESTUFA_ID,
        reservatorioId: 1,
        motivo: "Acionamento manual via dashboard",
        duracaoSegundos: 300,
        tipoAcionamento: 1
      }
    });
    
    if (resultado && resultado.id) {
      const status = document.getElementById("irrigationStatus");
      status.textContent = "Irrigação acionada manualmente";
      document.getElementById("manualMessage").textContent = "Bomba acionada. Durará até 5 minutos ou até desligar manualmente.";
      toast("Irrigação iniciada com sucesso!");
      
      // Simular parada após 5 minutos
      setTimeout(() => {
        status.textContent = "Irrigação em espera";
        document.getElementById("manualMessage").textContent = "O sistema voltou ao monitoramento normal.";
      }, 300000);
    }
  } catch (error) {
    toast("Erro ao iniciar irrigação");
  }
});

// ============================================================================
// ATUALIZAR DADOS
// ============================================================================

document.getElementById("refreshBtn").addEventListener("click", () => {
  loadDashboard();
  toast("Dados atualizados!");
});

// ============================================================================
// INICIALIZAÇÃO
// ============================================================================

// Carregar dados iniciais
makeChart();
loadDashboard();
loadSensores();
loadClima();

// Atualizar automaticamente a cada 30 segundos
setInterval(() => {
  loadDashboard();
}, 30000);

// Atualizar clima a cada 5 minutos
setInterval(() => {
  loadClima();
}, 300000);
