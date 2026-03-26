import { useState, useRef, useEffect } from 'react';
import { MessageCircle, X, Send, Bot, User, Loader2 } from 'lucide-react';

interface AiMessage {
  role: 'user' | 'ai';
  content: string;
  metrics?: {
    latency: number;
    tokens: number;
    cost: number;
  };
}

export default function AiChatWidget() {
  const [isOpen, setIsOpen] = useState(false);
  const [query, setQuery] = useState('');
  const [messages, setMessages] = useState<AiMessage[]>([
    { role: 'ai', content: 'Hola, soy el asistente de SmartUsers. ¿En qué te puedo ayudar sobre el sistema?' }
  ]);
  const [isLoading, setIsLoading] = useState(false);
  const [sessionId] = useState(() => crypto.randomUUID());

  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages, isLoading]);

  const handleSend = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!query.trim()) return;

    const userText = query;
    setQuery('');
    setMessages(prev => [...prev, { role: 'user', content: userText }]);
    setIsLoading(true);

    try {
      const response = await fetch('http://localhost:5003/api/aiagent/ask', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ question: userText, sessionId })
      });

      if (!response.ok) throw new Error('Error en la red');
      const json = await response.json();

      setMessages(prev => [...prev, { 
        role: 'ai', 
        content: json.data.answer,
        metrics: {
          latency: json.data.latencyMs,
          tokens: json.data.totalTokens,
          cost: json.data.estimatedCostUsd
        }
      }]);
    } catch (error) {
      setMessages(prev => [...prev, { role: 'ai', content: '❌ Ocurrió un error al contactar al agente.' }]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={{ position: 'fixed', bottom: '20px', right: '20px', zIndex: 9999 }}>
      {!isOpen && (
        <button 
          onClick={() => setIsOpen(true)}
          style={{ backgroundColor: 'blue', color: 'white', padding: '20px', borderRadius: '50px' }}
        >
          <MessageCircle size={28} />
        </button>
      )}

      {isOpen && (
        <div className="bg-white w-80 sm:w-96 rounded-2xl shadow-2xl border border-gray-200 flex flex-col overflow-hidden transition-all h-[500px]">
          <div className="bg-blue-600 text-white p-4 flex justify-between items-center">
            <div className="flex items-center gap-2">
              <Bot size={20} />
              <h3 className="font-bold">SmartUsers AI</h3>
            </div>
            <button onClick={() => setIsOpen(false)} className="hover:text-gray-200">
              <X size={20} />
            </button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-gray-50">
            {messages.map((msg, idx) => (
              <div key={idx} className={`flex flex-col ${msg.role === 'user' ? 'items-end' : 'items-start'}`}>
                <div className={`flex items-center gap-2 mb-1 ${msg.role === 'user' ? 'flex-row-reverse' : 'flex-row'}`}>
                  {msg.role === 'ai' ? <Bot size={16} className="text-blue-600" /> : <User size={16} className="text-gray-600" />}
                  <span className="text-xs text-gray-500 font-medium">{msg.role === 'ai' ? 'Asistente' : 'Tú'}</span>
                </div>
                <div className={`p-3 rounded-2xl max-w-[85%] text-sm ${
                  msg.role === 'user' ? 'bg-blue-600 text-white rounded-tr-none' : 'bg-white border border-gray-200 text-gray-800 rounded-tl-none shadow-sm'
                }`}>
                  {msg.content}
                </div>
                {msg.metrics && (
                  <div className="text-[10px] text-gray-400 mt-1 pl-1">
                    ⏱️ {msg.metrics.latency}ms | 🪙 {msg.metrics.tokens} tokens | 💰 ${msg.metrics.cost.toFixed(5)}
                  </div>
                )}
              </div>
            ))}
            {isLoading && (
              <div className="flex items-center gap-2 text-gray-400">
                <Loader2 className="animate-spin" size={16} />
                <span className="text-xs">El agente está pensando...</span>
              </div>
            )}
          </div>

          <form onSubmit={handleSend} className="p-3 bg-white border-t border-gray-100 flex gap-2">
            <input
              type="text"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              placeholder="Pregunta sobre SmartUsers..."
              className="flex-1 bg-gray-100 border-transparent focus:bg-white focus:border-blue-600 focus:ring-0 rounded-xl px-4 py-2 text-sm outline-none transition-all"
              disabled={isLoading}
            />
            <button 
              type="submit" 
              disabled={isLoading || !query.trim()}
              className="bg-blue-600 text-white p-2 rounded-xl hover:bg-blue-700 disabled:opacity-50 transition-all"
            >
              <Send size={18} />
            </button>
          </form>
        </div>
      )}
    </div>
  );
}