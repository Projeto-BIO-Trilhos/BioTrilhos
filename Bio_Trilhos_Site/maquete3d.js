import * as THREE from 'three';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';

const canvas = document.getElementById('threeCanvas');
if (!canvas) throw new Error('Canvas da maquete não encontrado');

const scene = new THREE.Scene();
scene.background = new THREE.Color(0xdfeae5);
scene.fog = new THREE.Fog(0xdfeae5, 24, 55);

const camera = new THREE.PerspectiveCamera(38, 1, 0.1, 100);
camera.position.set(18, 13, 20);

const renderer = new THREE.WebGLRenderer({ canvas, antialias: true, alpha: false });
renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
renderer.shadowMap.enabled = true;
renderer.shadowMap.type = THREE.PCFSoftShadowMap;
renderer.outputColorSpace = THREE.SRGBColorSpace;

const controls = new OrbitControls(camera, canvas);
controls.enableDamping = true;
controls.dampingFactor = 0.06;
controls.target.set(0, 2.8, 0);
controls.minDistance = 8;
controls.maxDistance = 38;
controls.maxPolarAngle = Math.PI * 0.49;

scene.add(new THREE.HemisphereLight(0xeaf6ff, 0x526b45, 2.2));
const sun = new THREE.DirectionalLight(0xfff3d6, 3.2);
sun.position.set(-12, 18, 10);
sun.castShadow = true;
sun.shadow.mapSize.set(2048, 2048);
Object.assign(sun.shadow.camera, { left:-22, right:22, top:22, bottom:-22 });
scene.add(sun);

const mat = (color, extra = {}) => new THREE.MeshStandardMaterial({ color, roughness: 0.72, metalness: 0.05, ...extra });
const box = (w,h,d,material,x=0,y=0,z=0) => { const m=new THREE.Mesh(new THREE.BoxGeometry(w,h,d),material);m.position.set(x,y,z);m.castShadow=true;m.receiveShadow=true;scene.add(m);return m; };
const cylinder = (rt,rb,h,material,x=0,y=0,z=0,segments=24) => { const m=new THREE.Mesh(new THREE.CylinderGeometry(rt,rb,h,segments),material);m.position.set(x,y,z);m.castShadow=true;m.receiveShadow=true;scene.add(m);return m; };

// Terreno e caminhos
box(34,.25,26,mat(0x91a985),0,-.14,0);
for(let i=-12;i<=12;i+=4) box(34,.018,.8,mat(0x9d9b88),0,.01,i);

// Trilhos ferroviários passando pelo interior
const railMat=mat(0x4d5552,{metalness:.78,roughness:.28});
box(25,.09,.11,railMat,0,.14,-2.1);
box(25,.09,.11,railMat,0,.14,-0.3);
for(let x=-12;x<=12;x+=1.15) box(.13,.12,2.25,mat(0x73533a),x,.05,-1.2);

// Estufa estrutural
const frameMat=mat(0xb9c6c1,{metalness:.62,roughness:.25});
const glassMat=mat(0xbfe1df,{transparent:true,opacity:.28,roughness:.12,metalness:.08,side:THREE.DoubleSide,depthWrite:false});
box(21,.12,.08,frameMat,0,6.9,7.5);
box(21,.12,.08,frameMat,0,6.9,-7.5);
for(let x=-10;x<=10;x+=2.5){
  const left=cylinder(.09,.09,7,frameMat,x,3.5,-7.5,12);
  left.rotation.z=0;
  cylinder(.09,.09,7,frameMat,x,3.5,7.5,12);
  cylinder(.09,.09,15.0,frameMat,x,6.9,0,12).rotation.x=Math.PI/2;
}
for(let z=-7.5;z<=7.5;z+=2.5){
  box(.12,7,.12,frameMat,-10.5,3.5,z); box(.12,7,.12,frameMat,10.5,3.5,z);
}
box(21,5.2,.025,glassMat,0,3.45,7.48);
box(21,5.2,.025,glassMat,0,3.45,-7.48);
box(.025,5.2,15,glassMat,-10.48,3.45,0);
box(.025,5.2,15,glassMat,10.48,3.45,0);
// Telhado de vidro inclinado
const roofGeo=new THREE.BoxGeometry(11.1,.08,16);
const roofLeft=new THREE.Mesh(roofGeo,glassMat); roofLeft.position.set(-5.25,6.55,0); roofLeft.rotation.z=-.62; roofLeft.castShadow=false; scene.add(roofLeft);
const roofRight=new THREE.Mesh(roofGeo,glassMat); roofRight.position.set(5.25,6.55,0); roofRight.rotation.z=.62; scene.add(roofRight);

// Trem
const trainGroup=new THREE.Group(); trainGroup.position.set(0,0,-1.2); scene.add(trainGroup);
const trainBody=new THREE.Mesh(new THREE.BoxGeometry(7.5,1.8,1.75),mat(0x2f6848,{roughness:.38,metalness:.25}));trainBody.position.set(0,1.35,0);trainBody.castShadow=true;trainGroup.add(trainBody);
const trainRoof=new THREE.Mesh(new THREE.BoxGeometry(7.8,.18,1.95),mat(0xe5e7df));trainRoof.position.set(0,2.28,0);trainGroup.add(trainRoof);
for(let x=-2.7;x<=2.7;x+=1.8){for(let z of [-.89,.89]){const win=new THREE.Mesh(new THREE.BoxGeometry(1.05,.72,.03),mat(0x9dc6d1,{transparent:true,opacity:.75}));win.position.set(x,1.55,z);trainGroup.add(win)}}
for(let x=-2.7;x<=2.7;x+=2.7){for(let z of [-.95,.95]){const wheel=new THREE.Mesh(new THREE.CylinderGeometry(.48,.48,.22,20),mat(0x303735,{metalness:.4}));wheel.position.set(x,.38,z);wheel.rotation.x=Math.PI/2;trainGroup.add(wheel)}}

// Vegetação Mata Atlântica
function createTree(x,z,scale=1){
  const g=new THREE.Group();g.position.set(x,.2,z);g.scale.setScalar(scale);scene.add(g);
  const trunk=new THREE.Mesh(new THREE.CylinderGeometry(.16,.28,2.4,10),mat(0x66503a));trunk.position.y=1.2;trunk.castShadow=true;g.add(trunk);
  const leafMats=[0x2f783f,0x3c8b4a,0x1f6640];
  for(let i=0;i<3;i++){const crown=new THREE.Mesh(new THREE.SphereGeometry(.95-i*.12,12,9),mat(leafMats[i]));crown.position.set((i-1)*.35,2.45+i*.2,(i%2)*.35);crown.scale.y=.85;crown.castShadow=true;g.add(crown)}
}
[[-8,-5,1.1],[8,-5,.95],[-8,5,1],[8,5,1.15],[12,-7,.8],[-12,7,.8]].forEach(v=>createTree(v[0],v[1],v[2]));

function createPlant(x,z,scale=1){
  const g=new THREE.Group();g.position.set(x,.1,z);g.scale.setScalar(scale);scene.add(g);
  for(let i=0;i<7;i++){const leaf=new THREE.Mesh(new THREE.SphereGeometry(.24,8,6),mat(i%2?0x397b43:0x55a05b));leaf.position.set(Math.cos(i)*.38,.42+Math.random()*.15,Math.sin(i)*.38);leaf.scale.set(.8,.22,1.7);leaf.rotation.z=.8;leaf.castShadow=true;g.add(leaf)}
}
for(let x=-8;x<=8;x+=2) for(let z of [3.8,5.7,-5.7,-3.8]) createPlant(x,z,.75);

// Canteiros elevados
for(let z of [4.6,-4.6]){
  box(16,.55,2.1,mat(0x6f5138),0,.26,z);
  for(let x=-7;x<=7;x+=1.4) createPlant(x,z,.58);
}

// Reservatório
const tank= cylinder(1.45,1.45,3.2,mat(0x4d7e68,{metalness:.25,roughness:.35}),-13,1.7,5,32);
const tankTop=cylinder(1.55,1.55,.12,mat(0xa8b8b1,{metalness:.5}),-13,3.35,5,32);
const water=cylinder(1.37,1.37,1.6,mat(0x63b9ca,{transparent:true,opacity:.78,roughness:.1}),-13,1.8,5,32);

// Tubulação de irrigação
const pipeMat=mat(0x2d8e67,{metalness:.45,roughness:.35});
for(let z of [3.8,-3.8]){ cylinder(.07,.07,15.5,pipeMat,0,2.15,z,10).rotation.z=Math.PI/2; for(let x=-6;x<=6;x+=2){ cylinder(.025,.025,.65,pipeMat,x,1.82,z,8); }}

const sensorObjects=[];
let selectedSensor=null;
let apiData={temperaturaAtual:24.8,umidadeArAtual:72,umidadeSoloAtual:64,reservatorioPercentual:82};
let sensorStatus={0:true,1:true,2:true,5:true};

function createSensor(x,y,z,label,valueKey,type,icon,color,sensorType){
  const g=new THREE.Group();g.position.set(x,y,z);scene.add(g);
  const pole=new THREE.Mesh(new THREE.CylinderGeometry(.045,.045,1.25,10),mat(0xd9e2dc,{metalness:.5}));pole.position.y=.62;pole.castShadow=true;g.add(pole);
  const head=new THREE.Mesh(new THREE.BoxGeometry(.55,.38,.24),mat(color,{metalness:.15,roughness:.3}));head.position.y=1.35;head.castShadow=true;g.add(head);
  const lamp=new THREE.Mesh(new THREE.SphereGeometry(.09,12,8),mat(0x65df85,{emissive:0x1da44a,emissiveIntensity:1}));lamp.position.set(.34,1.35,0);g.add(lamp);
  const halo=new THREE.Mesh(new THREE.RingGeometry(.38,.45,24),new THREE.MeshBasicMaterial({color:0x65df85,transparent:true,opacity:.65,side:THREE.DoubleSide}));halo.position.set(0,1.36,.13);halo.rotation.x=-Math.PI/2;g.add(halo);
  g.userData={sensor:true,label,valueKey,type,icon,sensorType,lamp,halo,active:true};sensorObjects.push(g);
  return g;
}

const temperatureSensor=createSensor(-5,0,2,'Sensor de Temperatura','temperaturaAtual','Temperatura','🌡️',0xf0aa63,0);
createSensor(5,0,5.7,'Sensor de Umidade do Ar','umidadeArAtual','Umidade do ar','💨',0x69aeb5,1);
createSensor(7,0,-5.2,'Sensor de Umidade do Solo','umidadeSoloAtual','Umidade do solo','🌱',0x65a56c,2);
createSensor(-13,0,2,'Sensor do Reservatório','reservatorioPercentual','Nível de água','💧',0x4a91aa,5);
selectedSensor=temperatureSensor.userData;
updateDetailFromApi(selectedSensor);

function formatSensor(key,value){
  if(key==='reservatorioPercentual'||key==='umidadeArAtual'||key==='umidadeSoloAtual')return `${Math.round(value)}%`;
  return `${Number(value).toFixed(1)}°C`;
}

function setSensorVisualState(sensor,active){
  sensor.userData.active=active;
  const lamp=sensor.userData.lamp;
  const halo=sensor.userData.halo;
  lamp.material.color.set(active?0x65df85:0xff7676);
  lamp.material.emissive.set(active?0x1da44a:0xa81414);
  lamp.material.emissiveIntensity=active?1:.15;
  halo.material.color.set(active?0x65df85:0xff7676);
  halo.visible=active;
}

function updateDetailFromApi(sensor){
  if(!sensor)return;
  const active=Boolean(sensorStatus[sensor.sensorType]);
  document.getElementById('detailIcon').textContent=sensor.icon;
  document.getElementById('detailName').textContent=sensor.label;
  document.getElementById('detailType').textContent=sensor.type;
  document.getElementById('detailValue').textContent=active?formatSensor(sensor.valueKey,apiData[sensor.valueKey]??0):'Sem leitura';
  const status=document.getElementById('detailStatus');
  status.textContent=active?'● Sensor ativo · atualizado pela API':'● Sensor inativo';
  status.style.color=active?'#4f9b68':'#c55252';
}

function setIrrigationVisualState(active){
  irrigationActive=Boolean(active);
  waterDrops.visible=irrigationActive;
  sprinklerObjects.forEach(nozzle=>{
    nozzle.material.emissive.set(irrigationActive?0x0d6e78:0x000000);
    nozzle.material.emissiveIntensity=irrigationActive?0.32:0;
  });

  const status=document.getElementById('maqueteIrrigationStatus');
  const button=document.getElementById('toggleMaqueteIrrigation');
  if(status&&button){
    status.textContent=irrigationActive?'Ativada':'Desligada';
    status.classList.toggle('active',irrigationActive);
    button.textContent=irrigationActive?'Desligar':'Acionar';
    button.classList.toggle('stop',irrigationActive);
  }
}

let activeIrrigationId=null;

async function toggleMaqueteIrrigation(){
  const button=document.getElementById('toggleMaqueteIrrigation');
  if(!button)return;
  button.disabled=true;

  try{
    if(irrigationActive){
      const body=activeIrrigationId!=null?String(activeIrrigationId):JSON.stringify(activeIrrigationId);
      await fetch(MAQUETE_API_BASE+'/irrigacao/parar',{method:'POST',headers:{'Content-Type':'application/json'},body});
      activeIrrigationId=null;
      setIrrigationVisualState(false);
    }else{
      const response=await fetch(MAQUETE_API_BASE+'/irrigacao/iniciar',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({estufaId:1,reservatorioId:1,motivo:'Acionamento pela maquete 3D',duracaoSegundos:300,tipoAcionamento:1})});
      if(!response.ok)throw new Error('Não foi possível iniciar a irrigação');
      const result=await response.json();
      activeIrrigationId=result.id??null;
      setIrrigationVisualState(true);
    }
  }catch(error){
    console.warn(error);
  }finally{
    button.disabled=false;
    refreshSensors();
  }
}

document.getElementById('toggleMaqueteIrrigation')?.addEventListener('click',toggleMaqueteIrrigation);

async function refreshSensors(){
  try{
    const [dashboardResponse,sensorsResponse]=await Promise.all([
      fetch(MAQUETE_API_BASE+'/dashboard/1'),
      fetch(MAQUETE_API_BASE+'/sensores?estufaId=1')
    ]);
    if(!dashboardResponse.ok||!sensorsResponse.ok)throw new Error('Falha na API');
    apiData=await dashboardResponse.json();
    setIrrigationVisualState(apiData.irrigacaoAtiva);
    const sensors=await sensorsResponse.json();
    sensorStatus={};
    sensors.forEach(sensor=>sensorStatus[Number(sensor.tipoSensor)]=Boolean(sensor.ativo));
    sensorObjects.forEach(sensor=>setSensorVisualState(sensor,Boolean(sensorStatus[sensor.userData.sensorType])));
    setIrrigationVisualState(apiData.irrigacaoAtiva);
    updateDetailFromApi(selectedSensor);
  }catch(error){
    console.warn('Erro ao atualizar sensores da maquete:',error);
    setIrrigationVisualState(false);
  }
}

const raycaster=new THREE.Raycaster();const pointer=new THREE.Vector2();
canvas.addEventListener('pointerdown',(event)=>{
  const rect=canvas.getBoundingClientRect();pointer.x=((event.clientX-rect.left)/rect.width)*2-1;pointer.y=-((event.clientY-rect.top)/rect.height)*2+1;
  raycaster.setFromCamera(pointer,camera);
  const hits=raycaster.intersectObjects(sensorObjects,true);
  if(!hits.length)return;
  let obj=hits[0].object;while(obj.parent&&!obj.userData.sensor)obj=obj.parent;
  if(obj.userData.sensor){
    selectedSensor=obj.userData;
    updateDetailFromApi(selectedSensor);
    document.querySelector('.sensor-detail').animate([{transform:'scale(.98)'},{transform:'scale(1)'}],{duration:220});
  }
});

const home={position:new THREE.Vector3(18,13,20),target:new THREE.Vector3(0,2.8,0)};
const greenhouse={position:new THREE.Vector3(10,8,10),target:new THREE.Vector3(0,3,-1)};
function moveCamera(view){controls.target.copy(view.target);camera.position.copy(view.position);controls.update();}
document.getElementById('viewHome')?.addEventListener('click',()=>moveCamera(home));
document.getElementById('viewGreenhouse')?.addEventListener('click',()=>moveCamera(greenhouse));
document.getElementById('toggleAutoRotate')?.addEventListener('click',(e)=>{controls.autoRotate=!controls.autoRotate;controls.autoRotateSpeed=.55;e.currentTarget.style.background=controls.autoRotate?'#dff3e6':'#ffffffed';});

document.querySelectorAll('.nav-item').forEach(item=>item.addEventListener('click',()=>{
  if(item.dataset.section==='maquete'){
    [50,150,300].forEach(delay=>setTimeout(resize,delay));
  }
}));

function resize(){
  const rect=canvas.getBoundingClientRect();
  if(rect.width<1||rect.height<1)return;
  renderer.setSize(rect.width,rect.height,false);
  camera.aspect=rect.width/rect.height;
  camera.updateProjectionMatrix();
}

const observer=new ResizeObserver(()=>resize());
observer.observe(canvas.parentElement);
window.addEventListener('resize',resize);
resize();

let started=performance.now();
function animate(now){
  requestAnimationFrame(animate);
  const t=(now-started)*.001;
  water.position.y=1.8+Math.sin(t*1.4)*.035;
  sensorObjects.forEach(sensor=>{
    if(sensor.userData.active){
      const pulse=1+Math.sin(t*3+sensor.position.x)*.12;
      sensor.userData.halo.scale.set(pulse,pulse,pulse);
      sensor.userData.halo.material.opacity=.45+Math.sin(t*3+sensor.position.x)*.18;
    }
  });
  controls.update();
  renderer.render(scene,camera);
  animateWaterDrops(t);
}
requestAnimationFrame(animate);

// Irrigadores e distribuição de água
const sprinklerObjects=[];
const waterDropMaterial=new THREE.PointsMaterial({
  color:0x78cfe8,
  size:.16,
  transparent:true,
  opacity:.92,
  depthWrite:false
});
const dropCount=120;
const dropPositions=new Float32Array(dropCount*3);
const dropOrigins=[];
const dropSpeeds=[];

for(let row=0;row<2;row++){
  const z=row===0?3.8:-3.8;
  for(let x=-6;x<=6;x+=2){
    const nozzle=cylinder(.09,.13,.38,mat(0x3b7f62,{metalness:.45,roughness:.3}),x,2.34,z,12);
    nozzle.rotation.z=0;
    sprinklerObjects.push(nozzle);
  }
}

for(let i=0;i<dropCount;i++){
  const row=i%2;
  const originX=-6+((i%7)*1.85);
  const originZ=row===0?3.8:-3.8;
  const origin={x:originX,z:originZ};
  dropOrigins.push(origin);
  dropPositions[i*3]=origin.x;
  dropPositions[i*3+1]=1.2+Math.random()*1.0;
  dropPositions[i*3+2]=origin.z+(Math.random()-.5)*1.2;
  dropSpeeds.push(.018+Math.random()*.018);
}
const dropGeometry=new THREE.BufferGeometry();
dropGeometry.setAttribute('position',new THREE.BufferAttribute(dropPositions,3));
const waterDrops=new THREE.Points(dropGeometry,waterDropMaterial);
waterDrops.visible=false;
scene.add(waterDrops);
let irrigationActive=false;

function animateWaterDrops(t){
  if(!irrigationActive||!waterDrops.visible)return;
  const positions=waterDrops.geometry.attributes.position.array;

  for(let i=0;i<dropCount;i++){
    positions[i*3]+=Math.sin(t*2+i*.7)*.0015;
    positions[i*3+1]-=dropSpeeds[i];
    positions[i*3+2]+=Math.cos(t*1.7+i*.5)*.001;

    if(positions[i*3+1]<.28){
      positions[i*3]=dropOrigins[i].x+(Math.random()-.5)*.9;
      positions[i*3+1]=2.35+Math.random()*1.45;
      positions[i*3+2]=dropOrigins[i].z+(Math.random()-.5)*1.15;
    }
  }
  waterDrops.geometry.attributes.position.needsUpdate=true;
}

const MAQUETE_API_BASE=(window.BIOTRILHOS_API_URL||'http://127.0.0.1:5000/api').replace(/\/$/,'');
refreshSensors();
setInterval(refreshSensors,10000);