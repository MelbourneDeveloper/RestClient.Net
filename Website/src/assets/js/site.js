const isChinese = document.documentElement.lang === 'zh';
const menu = document.querySelector('#mobile-nav');
const narrow = matchMedia('(max-width:700px)');
function syncMenu() { if (menu) { menu.open = !narrow.matches; menu.querySelector('summary').hidden = !narrow.matches; } }
syncMenu();
narrow.addEventListener('change', syncMenu);

const canvas = document.querySelector('#flow-canvas');
if (canvas && canvas.getContext('2d')) {
  const ctx = canvas.getContext('2d');
  canvas.hidden = false;
  const reduced = matchMedia('(prefers-reduced-motion:reduce)');
  const controls = [...document.querySelectorAll('[data-outcome]')];
  const toggle = document.querySelector('#motion-toggle');
  const status = document.querySelector('#outcome-status');
  const code = document.querySelector('#outcome-code code');
  const descriptions = isChinese ? [
    '200 OK → 成功结果中包含已反序列化的响应。',
    '404 Not Found → ResponseError 中保留响应数据、状态码与标头。',
    '连接失败 → ExceptionError 中保留异常，交由调用方明确处理。'
  ] : [
    '200 OK → Your deserialized response, wrapped in a typed success.',
    '404 Not Found → ResponseError preserves the response body, status code, and headers.',
    'Connection failed → ExceptionError carries the exception as an explicit result.'
  ];
  const branches = [
    'OkPost(var post) => post.Title,',
    'ErrorPost(ResponseErrorPost(var error, var status, _)) => $"HTTP {status}",',
    'ErrorPost(ExceptionErrorPost(var exception)) => exception.Message'
  ];
  let selected = 0, paused = reduced.matches, visible = true, frame = 0, time = 0, lastTime = 0, stopped = false;
  const colors = ['#d8ff62', '#ffbc82', '#9cb7ff'];
  const names = ['Ok<T>', 'ResponseError<E>', 'ExceptionError'];
  const ys = [95, 230, 365];
  function motionText() {
    toggle.textContent = isChinese ? (paused ? '播放动画' : '暂停动画') : (paused ? 'Play motion' : 'Pause motion');
    toggle.setAttribute('aria-pressed', String(paused));
    canvas.dataset.motion = paused ? 'paused' : 'running';
  }
  const round = (x,y,w,h,r=12) => {ctx.beginPath();ctx.roundRect(x,y,w,h,r);};
  const text = (value,x,y,size=13,color='#adb6a7',weight='400') => {
    ctx.fillStyle=color;ctx.font=`${weight} ${size}px ui-monospace, monospace`;ctx.fillText(value,x,y);
  };
  function route(i) {ctx.beginPath();ctx.moveTo(596,230);ctx.bezierCurveTo(750,230,715,ys[i],840,ys[i]);}
  function drawMobile() {
    ctx.clearRect(0,0,600,650);
    for(let x=20;x<600;x+=28) for(let y=20;y<650;y+=28) {ctx.fillStyle='#2a342a';ctx.fillRect(x,y,1,1);}
    ctx.strokeStyle=colors[selected];ctx.lineWidth=2;ctx.beginPath();ctx.moveTo(300,145);ctx.lineTo(300,500);ctx.stroke();
    for(let r=86;r<=120;r+=34) {ctx.beginPath();ctx.arc(300,318,r,0,Math.PI*2);ctx.strokeStyle='#344a2c';ctx.stroke();}
    ctx.save();ctx.translate(300,318);ctx.rotate(time*.14);ctx.setLineDash([8,15]);ctx.strokeStyle=colors[selected];ctx.beginPath();ctx.arc(0,0,120,0,Math.PI*2);ctx.stroke();ctx.restore();
    round(110,65,380,90);ctx.fillStyle='#121a15';ctx.fill();ctx.strokeStyle='#506448';ctx.stroke();text('HTTP REQUEST',135,98,17);text('GET /posts/1',135,132,26,'#edf0e8');
    ctx.shadowColor=colors[selected];ctx.shadowBlur=26;round(212,269,176,98,16);ctx.fillStyle='#1f2b1d';ctx.fill();ctx.shadowBlur=0;ctx.strokeStyle=colors[selected];ctx.stroke();text('Result',241,313,30,'#edf0e8','600');text('<T, E>',258,343,19,colors[selected]);
    round(60,492,480,98);ctx.fillStyle='#24301f';ctx.fill();ctx.strokeStyle=colors[selected];ctx.stroke();text(names[selected],87,534,25,colors[selected]);text(['200 · RESPONSE DATA','404 · BODY + STATUS','NETWORK · EXCEPTION'][selected],87,566,16);
    if(!paused) for(let n=0;n<3;n++){const y=155+(time*.19+n/3)%1*337;ctx.shadowColor=colors[selected];ctx.shadowBlur=16;ctx.beginPath();ctx.arc(300,y,5,0,Math.PI*2);ctx.fillStyle=colors[selected];ctx.fill();ctx.shadowBlur=0;}
    text('ONE CALL. EXPLICIT OUTCOMES.',90,630,15);
  }
  function draw() {
    const mobile = narrow.matches;
    const width = mobile ? 600 : 1200, height = mobile ? 650 : 470;
    if(canvas.width!==width || canvas.height!==height){canvas.width=width;canvas.height=height;}
    if(mobile){drawMobile();return;}
    ctx.clearRect(0,0,1200,470);
    for(let x=25;x<1200;x+=28) for(let y=20;y<470;y+=28) {ctx.fillStyle='#2a342a';ctx.fillRect(x,y,1,1);}
    // These are drawing commands for the illustrated request model, not DOM styling.
    ctx.strokeStyle='#334033';ctx.lineWidth=1;
    ctx.beginPath();ctx.moveTo(225,230);ctx.lineTo(466,230);ctx.stroke();
    for(let i=0;i<3;i++) {route(i);ctx.strokeStyle=i===selected?colors[i]:'#344137';ctx.lineWidth=i===selected?2:1;ctx.stroke();}
    for(let r=74;r<=130;r+=28) {ctx.beginPath();ctx.arc(530,230,r,0,Math.PI*2);ctx.strokeStyle=r===74?'#596944':'#28382a';ctx.lineWidth=1;ctx.stroke();}
    ctx.save();ctx.translate(530,230);ctx.rotate(time*.14);ctx.setLineDash([7,14]);ctx.strokeStyle='#5b713d';ctx.beginPath();ctx.arc(0,0,130,0,Math.PI*2);ctx.stroke();ctx.restore();
    ctx.shadowColor=colors[selected];ctx.shadowBlur=26;round(466,187,128,86,16);ctx.fillStyle='#1f2b1d';ctx.fill();ctx.shadowBlur=0;ctx.strokeStyle=colors[selected];ctx.stroke();
    text('Result',487,229,24,'#edf0e8','600');text('<T, E>',502,252,14,colors[selected]);
    round(32,189,192,82);ctx.fillStyle='#121a15';ctx.fill();ctx.strokeStyle='#506448';ctx.stroke();
    text('HTTP REQUEST',50,217,12);text('GET /posts/1',50,246,20,'#edf0e8');
    for(let i=0;i<3;i++) {
      round(842,ys[i]-37,314,74);ctx.fillStyle=i===selected?'#24301f':'#121a15';ctx.fill();ctx.strokeStyle=i===selected?colors[i]:'#344137';ctx.stroke();
      ctx.beginPath();ctx.arc(864,ys[i],4,0,Math.PI*2);ctx.fillStyle=colors[i];ctx.fill();
      text(names[i],881,ys[i]-2,19,i===selected?colors[i]:'#889882','500');
      text(['200 · RESPONSE DATA','404 · BODY + STATUS','NETWORK · EXCEPTION'][i],881,ys[i]+20,11);
    }
    if(!paused) for(let n=0;n<4;n++) {
      const p=(time*.19+n*.25)%1;
      let x,y;
      if(p<.5){x=224+p*2*242;y=230;} else {
        const t=(p-.5)*2,q=1-t;
        x=q*q*q*596+3*q*q*t*750+3*q*t*t*715+t*t*t*840;
        y=q*q*q*230+3*q*q*t*230+3*q*t*t*ys[selected]+t*t*t*ys[selected];
      }
      ctx.shadowColor=colors[selected];ctx.shadowBlur=16;ctx.beginPath();ctx.arc(x,y,4,0,Math.PI*2);ctx.fillStyle=colors[selected];ctx.fill();ctx.shadowBlur=0;
    }
    text('REQUEST',35,165,10);text('EXPLICIT OUTCOMES',843,35,10);text('ONE CALL. EVERY POSSIBILITY.',35,435,11);text('RESTCLIENT.NET / LIVE MODEL',900,435,10);
  }
  function tick(timestamp) {
    frame=0;
    if(paused || !visible || document.hidden || stopped) {lastTime=0;return;}
    time+=lastTime?Math.min((timestamp-lastTime)/1000,.05):0;
    lastTime=timestamp;draw();frame=requestAnimationFrame(tick);
  }
  function resume() {
    draw();motionText();
    if(!paused && visible && !document.hidden && !frame && !stopped) frame=requestAnimationFrame(tick);
    if(paused && frame) {cancelAnimationFrame(frame);frame=0;lastTime=0;}
  }
  controls.forEach((button,index)=>{
    button.disabled=false;
    button.addEventListener('click',()=>{
      selected=index;
      controls.forEach((item,i)=>item.setAttribute('aria-pressed',String(i===index)));
      status.textContent=descriptions[index];canvas.dataset.outcome=button.dataset.outcome;
      code.replaceChildren();
      code.append(document.createTextNode('// Selected branch: '+names[index]+'\nvar message = result switch\n{\n'));
      branches.forEach((branch,i)=>{const line=document.createElement(i===index?'strong':'span');line.textContent='    '+branch+'\n';code.append(line);});
      code.append(document.createTextNode('};'));resume();
    });
  });
  toggle.hidden=false;
  toggle.addEventListener('click',()=>{paused=!paused;resume();});
  reduced.addEventListener('change',()=>{paused=reduced.matches;resume();});
  narrow.addEventListener('change',resume);
  document.addEventListener('visibilitychange',resume);
  new IntersectionObserver(entries=>{visible=entries[0].isIntersecting;resume();}).observe(canvas);
  window.addEventListener('pagehide',()=>{stopped=true;if(frame)cancelAnimationFrame(frame);frame=0;lastTime=0;});
  window.addEventListener('pageshow',()=>{if(!stopped)return;stopped=false;frame=0;lastTime=0;resume();});
  canvas.dataset.outcome='success';resume();
}
